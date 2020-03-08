module ImageCartoonEffect


open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices
open ImageFxLib
open System



type SmoothingFilterType =
    | NoFilter
    | Gaussian3x3
    | Gaussian5x5
    | Gaussian7x7
    | Median3x3
    | Median5x5
    | Median7x7
    | Median9x9
    | Mean3x3
    | Mean5x5
    | LowPass3x3
    | LowPass5x5
    | Sharpen3x3



let SmoothingFilter (filterType:SmoothingFilterType) (sourceBitmap:Bitmap) =
    printfn "Filter: type=%A" filterType
    let inputBitmap =
        match filterType with
        | NoFilter -> sourceBitmap
        | Gaussian3x3 -> ConvolutionFilter (Matrix.Gaussian3x3) (1. / 16.) (0) sourceBitmap
        | Gaussian5x5 -> ConvolutionFilter (Matrix.Gaussian5x5) (1. / 159.) (0) sourceBitmap
        | Gaussian7x7 -> ConvolutionFilter (Matrix.Gaussian7x7) (1. / 136.) (0) sourceBitmap
        | Median3x3 -> MedianFilter (3) sourceBitmap
        | Median5x5 -> MedianFilter (5) sourceBitmap
        | Median7x7 -> MedianFilter (7) sourceBitmap
        | Median9x9 -> MedianFilter (9) sourceBitmap
        | Mean3x3 -> ConvolutionFilter (Matrix.Mean3x3) (1. / 9.)  (0) sourceBitmap
        | Mean5x5 -> ConvolutionFilter (Matrix.Mean5x5) (1. / 25.)  (0) sourceBitmap
        | LowPass3x3 -> ConvolutionFilter (Matrix.LowPass3x3) (1. / 16.) (0) sourceBitmap
        | LowPass5x5 -> ConvolutionFilter (Matrix.LowPass5x5) (1. / 60.) (0) sourceBitmap
        | Sharpen3x3 -> ConvolutionFilter (Matrix.Sharpen3x3) (1. / 8.) (0) sourceBitmap
    inputBitmap

let CartoonEffectFilter (filterType:SmoothingFilterType) (threshold:int) (sourceBitmap':Bitmap) =
    let sourceBitmap = SmoothingFilter filterType sourceBitmap'

    let sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    let pixelBuffer = Array.create (sourceData.Stride * sourceData.Height) 0uy
    let resultBuffer = Array.create (sourceData.Stride * sourceData.Height) 0uy
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)
    sourceBitmap.UnlockBits(sourceData)

    let mabs bval = Math.Abs(int bval)

    let offsetYs = [ for offsetY in 1..(sourceBitmap.Height-2) do yield offsetY ]
    let offsetXs = [ for offsetX in 1..(sourceBitmap.Width-2) do yield offsetX ]

    Seq.allPairs offsetYs offsetXs
    |> Seq.iter (fun (offy,offx) -> 
        let byteOffset = offy * sourceData.Stride + offx * 4

        let blueGradient' = mabs (pixelBuffer.[byteOffset-4] - pixelBuffer.[byteOffset+4])
        let blueGradient = blueGradient' + mabs (pixelBuffer.[byteOffset-sourceData.Stride] - pixelBuffer.[byteOffset+sourceData.Stride])
        
        let boff1 = byteOffset+1
        let greenGradient' = mabs (pixelBuffer.[boff1-4] - pixelBuffer.[boff1+4])
        let greenGradient = greenGradient' + mabs (pixelBuffer.[boff1-sourceData.Stride] - pixelBuffer.[boff1+sourceData.Stride])

        let boff2 = byteOffset+2
        let redGradient' = mabs (pixelBuffer.[boff2-4] - pixelBuffer.[boff2+4])
        let redGradient = redGradient' + mabs (pixelBuffer.[boff2-sourceData.Stride] - pixelBuffer.[boff2+sourceData.Stride])

        let exceedsThreshold =
            if (blueGradient + greenGradient + redGradient > threshold)
            then true   //printfn " (1)"; true
            else
                //byteOffset -= 2
                let blueGradient2 = mabs (pixelBuffer.[byteOffset-4] - pixelBuffer.[byteOffset+4])

                let greenGradient2 = mabs (pixelBuffer.[boff1-4] - pixelBuffer.[boff1+4])

                let redGradient2 = mabs (pixelBuffer.[boff2-4] - pixelBuffer.[boff2+4])

                if (blueGradient2 + greenGradient2 + redGradient2 > threshold)
                then printfn " (2)"; true
                else
                    //byteOffset -=2
                    let blueGradient3 = mabs (pixelBuffer.[byteOffset-sourceData.Stride] - pixelBuffer.[byteOffset+sourceData.Stride])

                    let greenGradient3 = mabs (pixelBuffer.[boff1-sourceData.Stride] - pixelBuffer.[boff1+sourceData.Stride])

                    let redGradient3 = mabs (pixelBuffer.[boff2-sourceData.Stride] - pixelBuffer.[boff2+sourceData.Stride])

                    if (blueGradient3 + greenGradient3 + redGradient3 > threshold)
                    then printfn " (3)"; true
                    else
                        let blueGradient4' = mabs (pixelBuffer.[byteOffset-4-sourceData.Stride] - pixelBuffer.[byteOffset+4+sourceData.Stride])
                        let blueGradient4 = blueGradient4' + mabs (pixelBuffer.[byteOffset-sourceData.Stride+4] - pixelBuffer.[byteOffset+sourceData.Stride-4])

                        let greenGradient4' = mabs (pixelBuffer.[boff1-4-sourceData.Stride] - pixelBuffer.[boff1+4+sourceData.Stride])
                        let greenGradient4 = greenGradient4' + mabs (pixelBuffer.[boff1-sourceData.Stride+4] - pixelBuffer.[boff1+sourceData.Stride-4])

                        let redGradient4' = mabs (pixelBuffer.[boff2-4-sourceData.Stride] - pixelBuffer.[boff2+4+sourceData.Stride])
                        let redGradient4 = redGradient4' + mabs (pixelBuffer.[boff2-sourceData.Stride+4] - pixelBuffer.[boff2+sourceData.Stride-4])

                        if (blueGradient4 + greenGradient4 + redGradient4 > threshold)
                        then printfn " (4)"; true
                        else false

        let mutable blue = 0
        let mutable green = 0
        let mutable red = 0

        //byteOffset -=2
        if (exceedsThreshold = false)
        then
            blue <- int (pixelBuffer.[byteOffset])
            green <- int (pixelBuffer.[byteOffset+1])
            red <- int (pixelBuffer.[byteOffset+2])

        //if (red+green+blue) > 0
        //then printf "exceedsThreshold:%b  r:%i g:%i b:%i       " exceedsThreshold red green blue

        let truncateRange x = match x with
                                | _ when x > 255 -> 255
                                | _ when x < 0 -> 0
                                | _ -> x

        blue <- truncateRange blue
        green <- truncateRange green
        red <- truncateRange red
        
        //if (red+green+blue) > 0
        //then printfn "r:%d g:%d b:%d" red green blue

        resultBuffer.[byteOffset] <- byte blue
        resultBuffer.[byteOffset+1] <- byte green
        resultBuffer.[byteOffset+2] <- byte red
        resultBuffer.[byteOffset+3] <- 255uy
        )

    let resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height)
    let resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
    Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length)
    resultBitmap.UnlockBits(resultData)
    resultBitmap

            
let applyFilter (filterType:SmoothingFilterType) (threshold:int) (img:Bitmap) = 
    CartoonEffectFilter filterType threshold img


let demo() =
    let dN' = __SOURCE_FILE__
    let demoName = dN'.[..dN'.Length-4]
    printfn "%s Start.....\n" demoName

    let tryFilter (filterType:SmoothingFilterType) =
        printfn "Filter: %A" filterType
        let filename = @"C:\Users\mhatm\Downloads\image2.jpg"
        let img = LoadImage filename
        let img' = applyFilter filterType 680 img
        let newFilename = @"C:\Users\mhatm\Downloads\image_modified.jpg"
        SaveNewImage newFilename img'
        BrowserOpen ("file:///" + newFilename)

    
    // try each of the filter types (winnow this down if you want a shorter demo)
    tryFilter (SmoothingFilterType.Gaussian3x3)
    tryFilter (SmoothingFilterType.Gaussian5x5)
    tryFilter (SmoothingFilterType.Gaussian7x7)
    tryFilter (SmoothingFilterType.Median3x3)
    (*tryFilter (SmoothingFilterType.Median5x5)
    tryFilter (SmoothingFilterType.Median7x7)
    tryFilter (SmoothingFilterType.Median9x9)
    tryFilter (SmoothingFilterType.Mean3x3)
    tryFilter (SmoothingFilterType.Mean5x5)
    tryFilter (SmoothingFilterType.LowPass3x3)
    tryFilter (SmoothingFilterType.LowPass5x5)
    tryFilter (SmoothingFilterType.Sharpen3x3)*)
    
    


    printfn "\n\n......%s End." demoName
