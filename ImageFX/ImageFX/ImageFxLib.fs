module ImageFxLib


open System.Drawing
open System.Drawing.Drawing2D
open System.Drawing.Imaging
open System.IO
open System.Diagnostics



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

