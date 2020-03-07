module BitmapShade


open System.Drawing
open System.IO
open System.Drawing.Imaging
open System.Runtime.InteropServices
open ImageFxLib

(*let mutable originalBitmap = null
let mutable previewBitmap = null
let mutable resultBitmap = null*)

//let mutable originalBitmap:Bitmap = null
//let mutable previewBitmap:Bitmap = null


// TODO: convert the 'for' loop into a better Seq operation (even better is parallel Seq)
let ColorShade (redShade:float, greenShade:float, blueShade:float) (sourceBitmap:Bitmap) =
    printfn "ColorShade: r=%.2f g=%.2f b=%.2f" redShade greenShade blueShade
    let sourceData =
        sourceBitmap.LockBits(
            new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    let pixelBuffer =
        Array.create (sourceData.Stride * sourceData.Height) 0uy

    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)

    sourceBitmap.UnlockBits(sourceData)

    //let red = 0.
    //let green = 0.
    //let blue = 0.

    let ks = [for k in 0..(pixelBuffer.Length-1)/4 do yield k*4]
    
    ks
    |> Seq.iter (fun k ->
        let blue = (byte (float pixelBuffer.[k] * blueShade))
        let green = (byte (float pixelBuffer.[k+1] * greenShade))
        let red = (byte (float pixelBuffer.[k+2] * redShade))
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
    ColorShade (red, green, blue) img


let demo() =
    printfn "BitmapShade Start..."
    let filename = @"C:\Users\mhatm\Downloads\image.png"
    let img = LoadImage filename

    (*let rv = 80.
    let gv = 80.
    let bv = 80.
    let img' = applyFilter (rv, gv, bv) img*)
    let rgb = (70., 70., 70.)
    let img' = applyFilter rgb img





    let newFilename = @"C:\Users\mhatm\Downloads\image_modified.jpg"
    SaveNewImage newFilename img'





    printfn "\n\n...BitmapShade Done."


