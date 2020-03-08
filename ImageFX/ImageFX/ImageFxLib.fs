module ImageFxLib


open System.Drawing
open System.Drawing.Drawing2D
open System.Drawing.Imaging
open System.IO
open System.Diagnostics
open System.Runtime.InteropServices
open System



/// Execute a bash command
let Bash (cmd:string) : string =
    let escapedArgs = cmd.Replace("\"", "\\\"")
    let mutable process = new Process()
    let mutable startInfo = new ProcessStartInfo()
    startInfo.FileName <- "/bin/bash"
    startInfo.Arguments <- "-c \"" + escapedArgs + "\""
    startInfo.RedirectStandardOutput <- true
    startInfo.UseShellExecute <- false
    startInfo.CreateNoWindow <- true
    process.StartInfo <- startInfo
    process.Start()
    let result = process.StandardOutput.ReadToEnd()
    process.WaitForExit()
    result
    
let BrowserOpen (filename:string) =
    printfn "'%s'" filename
    //ShellOpen "MicrosoftEdgeLauncher.exe " + filename
    let mutable startInfo = new ProcessStartInfo()
    //startInfo.FileName <- @"C:\Program Files\Internet Explorer\iexplore.exe"
    startInfo.FileName <- @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
    startInfo.Arguments <- filename
    startInfo.RedirectStandardOutput <- true
    startInfo.UseShellExecute <- false
    startInfo.CreateNoWindow <- true
    let mutable process = new Process()
    process.StartInfo <- startInfo
    process.Start()
    //let result = process.StandardOutput.ReadToEnd()
    //process.WaitForExit()
    ()

(*let ShellOpen (filename:string) =
    //let cmd = @"MicrosoftEdgeLauncher.exe file:///C:/something/index.html"
    let cAction = "open"
    let cFilename = filename
    //ShellExecute(0, cAction, cFilename, "", "", 1)
    ()*)

// TODO: add exception handling for load file
let LoadImage (filename:string) =
    use streamReader = new StreamReader(filename)
    let bitmap = Bitmap (Bitmap.FromStream(streamReader.BaseStream))
    streamReader.Close()
    //previewBitmap <- originalBitmap.CopyToSquareCanvas(picPreview.Width)
    //picPreview.Image = previewBitmap
    bitmap

// TODO: add exception handling for save file
let SaveNewImage (filename:string) (img:Bitmap) =
    let fileExtension = Path.GetExtension(filename).ToUpper()
    let imgFormat =
        match fileExtension with
        | "BMP" -> ImageFormat.Bmp
        | "JPG" -> ImageFormat.Jpeg
        | _ -> ImageFormat.Png   
    use streamWriter = new StreamWriter(filename, false)
    img.Save(streamWriter.BaseStream, imgFormat)
    streamWriter.Flush()
    streamWriter.Close()
    ()

let CopyToSquareCanvas (sourceBitmap:Bitmap) (canvasWidthLength:int) =
    //let ratio = 1.
    let maxSide =
        if sourceBitmap.Width > sourceBitmap.Height
        then sourceBitmap.Width
        else sourceBitmap.Height

    let ratio = (float maxSide) / (float canvasWidthLength)

    let bitmapResult =
        if sourceBitmap.Width > sourceBitmap.Height
        then new Bitmap(canvasWidthLength, int (float sourceBitmap.Height / ratio))
        else new Bitmap(int (float sourceBitmap.Width / ratio), canvasWidthLength)

    use graphicsResult = Graphics.FromImage(bitmapResult)
    graphicsResult.CompositingQuality = CompositingQuality.HighQuality
    graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic
    graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality
    graphicsResult.DrawImage(sourceBitmap,
        new Rectangle(0, 0, bitmapResult.Width, bitmapResult.Height),
        new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
        GraphicsUnit.Pixel)
    graphicsResult.Flush()
    bitmapResult



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












