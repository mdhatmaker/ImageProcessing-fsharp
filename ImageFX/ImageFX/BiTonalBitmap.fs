module BiTonalBitmap


open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices
open ImageFxLib


// TODO: convert the 'for' loop into a better Seq operation (even better is parallel Seq)
// where threshold is int [0..765]
let Bitonal (darkColor:Color, lightColor:Color, threshold:int) (sourceBitmap:Bitmap) =
    printfn "Bitonal: %A   %A   %i" darkColor lightColor threshold
    let sourceData =
        sourceBitmap.LockBits(
            new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    let pixelBuffer =
        Array.create (sourceData.Stride * sourceData.Height) 0uy
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)
    sourceBitmap.UnlockBits(sourceData)

    let ks = [for k in 0..(pixelBuffer.Length-1)/4 do yield k*4]

    let length =
        ks
        |> Seq.filter (fun k ->
            let sum = (pixelBuffer.[k] + pixelBuffer.[k+1] + pixelBuffer.[k+2])
            (int sum) <= threshold)
        |> Seq.length
    printfn "dark/light:  %.1f %%" ((float length) / (float ks.Length) * 100.)

    ks
    |> Seq.iter (fun k ->
        let sum = (pixelBuffer.[k] + pixelBuffer.[k+1] + pixelBuffer.[k+2])
        if (int sum) <= threshold
        then
            pixelBuffer.[k] <- darkColor.B
            pixelBuffer.[k+1] <- darkColor.G
            pixelBuffer.[k+2] <- darkColor.R
        else
            //printfn "%i  %i" (int sum) threshold
            pixelBuffer.[k] <- lightColor.B
            pixelBuffer.[k+1] <- lightColor.G
            pixelBuffer.[k+2] <- lightColor.R
        )



    let resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height)
    let resultData =
        resultBitmap.LockBits(
            new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
    Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length)
    resultBitmap.UnlockBits(resultData)
    resultBitmap

let applyFilter (darkColor:Color, lightColor:Color, threshold:int) (img:Bitmap) = 
    Bitonal (darkColor, lightColor, threshold) img


let demo() =
    let dN' = __SOURCE_FILE__
    let demoName = dN'.[..dN'.Length-4]
    printfn "%s Start.....\n" demoName

    let filename = @"C:\Users\mhatm\Downloads\image.png"
    let img = LoadImage filename

    let dark = Color.Black
    let light = Color.LightBlue
    let threshold = 180     // 0..765
    let img' = applyFilter (dark, light, threshold) img

    let newFilename = @"C:\Users\mhatm\Downloads\image_modified.jpg"
    SaveNewImage newFilename img'
    BrowserOpen ("file:///" + newFilename)



    printfn "\n\n......%s End." demoName

