namespace chapter_3
    module simple_types_and_literals =

        // squareAndAdd1 defaults to a:int -> b:int -> int
        let squareAndAdd1 a b = a * a + b

       // squareAndAdd2 defaults to a:float -> b:float -> float
       // because parameter 'a' was defined as a float. 
        let squareAndAdd2 (a: float) b = a * a + b

        // personally, I would rather declare my intent
        let squareAndAdd3 (a: float) (b: float) = a * a + b

        let run() = 
            printfn "%d" (squareAndAdd1 3 4)
            printfn "%f" (squareAndAdd2 3.0 4.0)
            printfn "%f" (squareAndAdd3 3.0 4.0)

    module arithmetic_conversions =

        let run() = 
            printfn "%d" (int 17.8)
            printfn "%d" (int -17.8)
            printfn "%A" (string 65)
            printfn "%f" (float 65)

    module simple_strings =

        let run() = 
            let s0 = "Couldn't put Humpty"
            printfn "%d" s0.Length
            printfn "%c" s0.[13]
            printfn "%s" s0.[13..16]

            // strings are immutable
            //s.[13] <- 'h'   // error FS0751: Invalid indexer expression
                            // error FS0810: Property 'Chars' cannot be set
                            // error FS0257: Invalid mutation of a constant expression.

            // simplest way to build strings is via concatenation using the + operator
            let s1 = "Couldn't put Humpty" + " " + "together again"
            printfn "%s" s1



