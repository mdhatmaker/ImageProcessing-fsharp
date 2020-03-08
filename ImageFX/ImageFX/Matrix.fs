﻿module Matrix




let Mean3x3 = [|
    [| 1.; 1.; 1.; |];
    [| 1.; 1.; 1.; |];
    [| 1.; 1.; 1.; |];
    |]

let Mean5x5 = [|
    [| 1.; 1.; 1.; 1.; 1.; |];
    [| 1.; 1.; 1.; 1.; 1.; |];
    [| 1.; 1.; 1.; 1.; 1.; |];
    [| 1.; 1.; 1.; 1.; 1.; |];
    [| 1.; 1.; 1.; 1.; 1.; |];
    |]


let Mean7x7 = [|
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    |]

let Mean9x9 = [|
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.;|];
    |]

let Gaussian3x3 = [|
    [| 1.; 2.; 1.; |];
    [| 2.; 4.; 2.; |];
    [| 1.; 2.; 1.; |];
    |]

let Gaussian5x5 = [|
    [| 2.; 4.; 5.; 4.; 2.; |];
    [| 4.; 9.; 12.; 9.; 4.; |];
    [| 5.; 12.; 15.; 12.; 5.; |];
    [| 4.; 9.; 12.; 9.; 4.; |];
    [| 2.; 4.; 5.; 4.; 2.; |];
    |]

let Gaussian7x7 = [|
    [| 1.; 1.; 2.; 2.; 2.; 1.; 1.; |];
    [| 1.; 2.; 2.; 4.; 2.; 2.; 1.; |];
    [| 2.; 2.; 4.; 8.; 4.; 2.; 2.; |];
    [| 2.; 4.; 8.; 16.; 8.; 4.; 2.; |];
    [| 2.; 2.; 4.; 8.; 4.; 2.; 2.; |];
    [| 1.; 2.; 2.; 4.; 2.; 2.; 1.; |];
    [| 1.; 1.; 2.; 2.; 2.; 1.; 1.; |];
    |]

let MotionBlur5x5 = [|
    [| 1.; 0.; 0.; 0.; 1.; |];
    [| 0.; 1.; 0.; 1.; 0.; |];
    [| 0.; 0.; 1.; 0.; 0.; |];
    [| 0.; 1.; 0.; 1.; 0.; |];
    [| 1.; 0.; 0.; 0.; 1.; |];
    |]

let MotionBlur5x5At45Degrees = [|
    [| 0.; 0.; 0.; 0.; 1.; |];
    [| 0.; 0.; 0.; 1.; 0.; |];
    [| 0.; 0.; 1.; 0.; 0.; |];
    [| 0.; 1.; 0.; 0.; 0.; |];
    [| 1.; 0.; 0.; 0.; 0.; |];
    |]

let MotionBlur5x5At135Degrees = [|
    [| 1.; 0.; 0.; 0.; 0.; |];
    [| 0.; 1.; 0.; 0.; 0.; |];
    [| 0.; 0.; 1.; 0.; 0.; |];
    [| 0.; 0.; 0.; 1.; 0.; |];
    [| 0.; 0.; 0.; 0.; 1.; |];
    |]

let MotionBlur7x7 = [|
    [| 1.; 0.; 0.; 0.; 0.; 0.; 1.; |];
    [| 0.; 1.; 0.; 0.; 0.; 1.; 0.; |];
    [| 0.; 0.; 1.; 0.; 1.; 0.; 0.; |];
    [| 0.; 0.; 0.; 1.; 0.; 0.; 0.; |];
    [| 0.; 0.; 1.; 0.; 1.; 0.; 0.; |];
    [| 0.; 1.; 0.; 0.; 0.; 1.; 0.; |];
    [| 1.; 0.; 0.; 0.; 0.; 0.; 1.; |];
    |]

let MotionBlur7x7At45Degrees = [|
    [| 0.; 0.; 0.; 0.; 0.; 0.; 1.; |];
    [| 0.; 0.; 0.; 0.; 0.; 1.; 0.; |];
    [| 0.; 0.; 0.; 0.; 1.; 0.; 0.; |];
    [| 0.; 0.; 0.; 1.; 0.; 0.; 0.; |];
    [| 0.; 0.; 1.; 0.; 0.; 0.; 0.; |];
    [| 0.; 1.; 0.; 0.; 0.; 0.; 0.; |];
    [| 1.; 0.; 0.; 0.; 0.; 0.; 0.; |];
    |]


let MotionBlur7x7At135Degrees = [|
    [| 1.; 0.; 0.; 0.; 0.; 0.; 0.; |];
    [| 0.; 1.; 0.; 0.; 0.; 0.; 0.; |];
    [| 0.; 0.; 1.; 0.; 0.; 0.; 0.; |];
    [| 0.; 0.; 0.; 1.; 0.; 0.; 0.; |];
    [| 0.; 0.; 0.; 0.; 1.; 0.; 0.; |];
    [| 0.; 0.; 0.; 0.; 0.; 1.; 0.; |];
    [| 0.; 0.; 0.; 0.; 0.; 0.; 1.; |];
    |]


let MotionBlur9x9 = [|
    [| 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.;|];
    [| 0.; 1.; 0.; 0.; 0.; 0.; 0.; 1.; 0.;|];
    [| 0.; 0.; 1.; 0.; 0.; 0.; 1.; 0.; 0.;|];
    [| 0.; 0.; 0.; 1.; 0.; 1.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 1.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 1.; 0.; 1.; 0.; 0.; 0.;|];
    [| 0.; 0.; 1.; 0.; 0.; 0.; 1.; 0.; 0.;|];
    [| 0.; 1.; 0.; 0.; 0.; 0.; 0.; 1.; 0.;|];
    [| 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.;|];
    |]


let MotionBlur9x9At45Degrees = [|
    [| 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.;|];
    [| 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.; 0.;|];
    [| 0.; 0.; 0.; 0.; 0.; 0.; 1.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 0.; 1.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 1.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 1.; 0.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 1.; 0.; 0.; 0.; 0.; 0.; 0.;|];
    [| 0.; 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.;|];
    [| 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.;|];
    |]


let MotionBlur9x9At135Degrees = [|
    [| 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.;|];
    [| 0.; 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 1.; 0.; 0.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 1.; 0.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 1.; 0.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 0.; 1.; 0.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 0.; 0.; 1.; 0.; 0.;|];
    [| 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.; 0.;|];
    [| 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.;|];
    |]


let LowPass3x3 = [|
    [| 1.; 2.; 1.;|];
    [| 2.; 4.; 2.;|];
    [| 1.; 2.; 1.;|];
    |]

let LowPass5x5 = [|
    [| 1.; 1.; 1.; 1.; 1.;|];
    [| 1.; 4.; 4.; 4.; 1.;|];
    [| 1.; 4.; 12.; 4.; 1.;|];
    [| 1.; 4.; 4.; 4.; 1.;|];
    [| 1.; 1.; 1.; 1.; 1.;|];
    |]

let Sharpen3x3 = [|
    [| -1.; -2.; -1.; |];
    [| 2.; 4.; 2.; |];
    [| 1.; 2.; 1.; |];
    |]






