// ImageProcessing in F# (mostly converted from C#)

open System


[<EntryPoint>]
let main argv =
    printfn "----- ImageProcessing in F# -----\n"

    //BitmapShade.demo()
    //BitmapTint.demo()
    //BiTonalBitmap.demo()
    ImageBlur.demo()


    0 // return an integer exit code
