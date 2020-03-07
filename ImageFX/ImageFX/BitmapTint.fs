module BitmapTint


open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices
open ImageFxLib


// TODO: convert the 'for' loop into a better Seq operation (even better is parallel Seq)
let ColorTint (redTint:float, greenTint:float, blueTint:float) (sourceBitmap:Bitmap) =
    printfn "ColorTint: r=%.2f g=%.2f b=%.2f" redTint greenTint blueTint
    let sourceData =
        sourceBitmap.LockBits(
            new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    let pixelBuffer =
        Array.create (sourceData.Stride * sourceData.Height) 0uy
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)
    sourceBitmap.UnlockBits(sourceData)

    let ks = [for k in 0..(pixelBuffer.Length-1)/4 do yield k*4]
    
    ks
    |> Seq.iter (fun k ->
        let blue = byte ((float (255uy - pixelBuffer.[k])) * blueTint)
        let green = byte ((float (255uy - pixelBuffer.[k+1])) * greenTint)
        let red = byte ((float (255uy - pixelBuffer.[k+2])) * redTint)
        pixelBuffer.[k] <- blue
        pixelBuffer.[k+1] <- green
        pixelBuffer.[k+2] <- red
        )

    let resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height)
    let resultData =
        resultBitmap.LockBits(
            new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
    Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length)
    resultBitmap.UnlockBits(resultData)
    resultBitmap

// where rv, gv, bv are floats 0..100
let applyFilter (rv:float, gv:float, bv:float) (img:Bitmap) = 
    let red = rv / 100.
    let green = gv / 100.
    let blue = bv / 100.
    ColorTint (red, green, blue) img


let demo() =
    let dN' = __SOURCE_FILE__
    let demoName = dN'.[..dN'.Length-4]
    printfn "%s Start.....\n" demoName

    let filename = @"C:\Users\mhatm\Downloads\image.png"
    let img = LoadImage filename

    let rgb = (100., 100., 100.)
    let img' = applyFilter rgb img

    let newFilename = @"C:\Users\mhatm\Downloads\image_modified.jpg"
    SaveNewImage newFilename img'
    BrowserOpen ("file:///" + newFilename)



    printfn "\n\n......%s End." demoName

