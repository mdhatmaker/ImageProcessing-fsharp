module ImageBlur


open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices
open ImageFxLib
open System


type BlurType =
    | GaussianBlur3x3
    | GaussianBlur5x5
    | Mean3x3
    | Mean5x5
    | Mean7x7
    | Mean9x9
    | Median3x3
    | Median5x5
    | Median7x7
    | Median9x9
    | Median11x11
    | MotionBlur5x5
    | MotionBlur5x5At135Degrees
    | MotionBlur5x5At45Degrees
    | MotionBlur7x7
    | MotionBlur7x7At135Degrees
    | MotionBlur7x7At45Degrees
    | MotionBlur9x9
    | MotionBlur9x9At135Degrees
    | MotionBlur9x9At45Degrees



let ConvolutionFilter (filterMatrix:float[][]) (factor:float) (bias:int) (sourceBitmap:Bitmap) = 
    let sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    let pixelBuffer = Array.create (sourceData.Stride * sourceData.Height) 0uy
    let resultBuffer = Array.create (sourceData.Stride * sourceData.Height) 0uy
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)
    sourceBitmap.UnlockBits(sourceData)

    let filterHeight = filterMatrix.GetLength(0)
    let filterWidth = filterMatrix.[0].GetLength(0)
    let filterOffset = (filterWidth - 1) / 2
    //let calcOffset = 0
    //let byteOffset = 0

    let offsetYs = [ for offsetY in filterOffset..(sourceBitmap.Height-filterOffset-1) do yield offsetY ]
    let offsetXs = [ for offsetX in filterOffset..(sourceBitmap.Width-filterOffset-1) do yield offsetX ]

    Seq.allPairs offsetYs offsetXs
    |> Seq.iter (fun (offy,offx) -> 
        let mutable blue = 0.
        let mutable green = 0.
        let mutable red = 0.

        let byteOffset = offy * sourceData.Stride + offx * 4

        let filterYs = [ for filterY in -filterOffset..(filterOffset-1) do yield filterY ]
        let filterXs = [ for filterX in -filterOffset..(filterOffset-1) do yield filterX ]

        Seq.allPairs filterYs filterXs
        |> Seq.iter (fun (filty,filtx) -> 
            let calcOffset = byteOffset + (filtx * 4) + (filty * sourceData.Stride)
            blue <- blue + (float pixelBuffer.[calcOffset]) * filterMatrix.[filty + filterOffset].[filtx + filterOffset]
            green <- green + (float pixelBuffer.[calcOffset+1]) * filterMatrix.[filty + filterOffset].[filtx + filterOffset]
            red <- red + (float pixelBuffer.[calcOffset+2]) * filterMatrix.[filty + filterOffset].[filtx + filterOffset]
            )

        blue <- factor * blue + (float bias)
        green <- factor * green + (float bias)
        red <- factor * red + (float bias)

        let truncateRange x = match x with
                              | _ when x > 255. -> 255.
                              | _ when x < 0. -> 0.
                              | _ -> x

        blue <- truncateRange blue
        green <- truncateRange green
        red <- truncateRange red

        resultBuffer.[byteOffset] <- (byte blue)
        resultBuffer.[byteOffset+1] <- (byte green)
        resultBuffer.[byteOffset+2] <- (byte red)
        resultBuffer.[byteOffset+3] <- 255uy
        )

    let resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height)
    let resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
    Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length)
    resultBitmap.UnlockBits(resultData)
    resultBitmap


let MedianFilter (matrixSize:int) (sourceBitmap:Bitmap) = 
    let sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    let pixelBuffer = Array.create (sourceData.Stride * sourceData.Height) 0uy
    let resultBuffer = Array.create (sourceData.Stride * sourceData.Height) 0uy
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)
    sourceBitmap.UnlockBits(sourceData)

    let filterOffset = (matrixSize - 1) / 2

    let mutable neighborPixels = List.init 0 (fun x -> 0)
    //let mutable middlePixel = Array.create 0 0uy

    let offsetYs = [ for offsetY in filterOffset..(sourceBitmap.Height-filterOffset-1) do yield offsetY ]
    let offsetXs = [ for offsetX in filterOffset..(sourceBitmap.Width-filterOffset-1) do yield offsetX ]

    Seq.allPairs offsetYs offsetXs
    |> Seq.iter (fun (offy,offx) -> 
        let byteOffset = offy * sourceData.Stride + offx * 4
        
        neighborPixels <- List.empty

        let filterYs = [ for filterY in -filterOffset..(filterOffset-1) do yield filterY ]
        let filterXs = [ for filterX in -filterOffset..(filterOffset-1) do yield filterX ]

        Seq.allPairs filterYs filterXs
        |> Seq.iter (fun (filty,filtx) -> 
            let calcOffset = byteOffset + (filtx * 4) + (filty * sourceData.Stride)
            neighborPixels <- List.append neighborPixels [BitConverter.ToInt32(pixelBuffer, calcOffset)]
            )

        neighborPixels <- List.sort neighborPixels

        let middlePixel = BitConverter.GetBytes(neighborPixels.[filterOffset])

        resultBuffer.[byteOffset] <- middlePixel.[0]
        resultBuffer.[byteOffset+1] <- middlePixel.[1]
        resultBuffer.[byteOffset+2] <- middlePixel.[2]
        resultBuffer.[byteOffset+3] <- middlePixel.[3]
        )

    let resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height)
    let resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
    Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length)
    resultBitmap.UnlockBits(resultData)
    resultBitmap


let ImageBlurFilter (blurType:BlurType) (sourceBitmap:Bitmap) =
    printfn "ImageBlur: blurtype=%A" blurType
    let resultBitmapFun =
        match blurType with
        | Mean3x3 -> ConvolutionFilter (Matrix.Mean3x3) (1. / 9.)  (0)
        | Mean5x5 -> ConvolutionFilter (Matrix.Mean5x5) (1. / 25.)  (0)
        | Mean7x7 -> ConvolutionFilter (Matrix.Mean7x7) (1. / 49.)  (0)
        | Mean9x9 -> ConvolutionFilter (Matrix.Mean9x9) (1. / 81.)  (0)     
        | GaussianBlur3x3 -> ConvolutionFilter (Matrix.Gaussian3x3) (1. / 16.)  (0)
        | GaussianBlur5x5 -> ConvolutionFilter (Matrix.Gaussian5x5) (1. / 159.)  (0)
        | MotionBlur5x5 -> ConvolutionFilter (Matrix.MotionBlur5x5) (1. / 10.)  (0)
        | MotionBlur5x5At45Degrees  -> ConvolutionFilter (Matrix.MotionBlur5x5At45Degrees) (1. / 5.)  (0)
        | MotionBlur5x5At135Degrees -> ConvolutionFilter (Matrix.MotionBlur5x5At135Degrees) (1. / 5.)  (0)
        | MotionBlur7x7 -> ConvolutionFilter (Matrix.MotionBlur7x7) (1. / 14.)  (0)
        | MotionBlur7x7At45Degrees -> ConvolutionFilter (Matrix.MotionBlur7x7At45Degrees) (1. / 7.)  (0)
        | MotionBlur7x7At135Degrees -> ConvolutionFilter (Matrix.MotionBlur7x7At135Degrees) (1. / 7.)  (0)
        | MotionBlur9x9 -> ConvolutionFilter (Matrix.MotionBlur9x9) (1. / 18.)  (0)
        | MotionBlur9x9At45Degrees -> ConvolutionFilter (Matrix.MotionBlur9x9At45Degrees) (1. / 9.)  (0)
        | MotionBlur9x9At135Degrees -> ConvolutionFilter (Matrix.MotionBlur9x9At135Degrees) (1. / 9.)  (0)
        | Median3x3 -> MedianFilter (3)
        | Median5x5 -> MedianFilter (5)
        | Median7x7 -> MedianFilter (7)
        | Median9x9 -> MedianFilter (9)
        | Median11x11 -> MedianFilter (11)
    resultBitmapFun sourceBitmap


let applyFilter (blurType:BlurType) (img:Bitmap) = 
    ImageBlurFilter blurType img


let demo() =
    let dN' = __SOURCE_FILE__
    let demoName = dN'.[..dN'.Length-4]
    printfn "%s Start.....\n" demoName

    let tryFilter (blurType:BlurType) =
        printfn "Filter: %A" blurType
        let filename = @"C:\Users\mhatm\Downloads\image1.jpg"
        let img = LoadImage filename
        let img' = applyFilter blurType img
        let newFilename = @"C:\Users\mhatm\Downloads\image_modified.jpg"
        SaveNewImage newFilename img'
        BrowserOpen ("file:///" + newFilename)

    
    // try each of the filter types (winnow this down if you want a shorter demo)
    tryFilter (BlurType.Mean3x3)
    tryFilter (BlurType.Mean5x5)
    tryFilter (BlurType.Mean7x7)
    tryFilter (BlurType.Mean9x9)
    tryFilter (BlurType.GaussianBlur3x3)
    tryFilter (BlurType.GaussianBlur5x5)
    tryFilter (BlurType.MotionBlur5x5)
    tryFilter (BlurType.MotionBlur5x5At45Degrees)
    tryFilter (BlurType.MotionBlur5x5At135Degrees)
    tryFilter (BlurType.MotionBlur7x7)
    tryFilter (BlurType.MotionBlur7x7At45Degrees)
    tryFilter (BlurType.MotionBlur7x7At135Degrees)
    tryFilter (BlurType.MotionBlur9x9)
    tryFilter (BlurType.MotionBlur9x9At45Degrees)
    tryFilter (BlurType.MotionBlur9x9At135Degrees)
    tryFilter (BlurType.Median3x3)
    tryFilter (BlurType.Median5x5)
    tryFilter (BlurType.Median7x7)
    tryFilter (BlurType.Median9x9)
    tryFilter (BlurType.Median11x11)


    printfn "\n\n......%s End." demoName

