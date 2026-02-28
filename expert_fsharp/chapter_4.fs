module chapter_4

    /// Collection of code used in most examples in Chapter 4
    module http_stuff =

        open System.Net.Http
        open System.Threading.Tasks

        /// Get the contents of the URL via a web request using HttpClient (async)
        let http (url: string) =
            use client = new HttpClient()
            let task: Task<string> = client.GetStringAsync(url)
            task.Result

        let fetch url = 
            try Some (http url)
            with :? System.Net.WebException -> None

    module time_stuff =
        // from Chapter 2, but modified to use stop watch.
        let time f = 
            let sw = System.Diagnostics.Stopwatch.StartNew()
            let res = f()
            let finish = sw.Stop()
            (res, sw.Elapsed.TotalMilliseconds |> sprintf "%f ms")
        
    module imperative_looping_and_iterating =

        open System

        module simple_for_loops =

            let repeatFetch url n =
                for i = 1 to n do
                    let html = http_stuff.http url
                    printf "fetched <<< %s >>>\n" html
                printf "Done!\n"
            
        module simple_while_loops =

            let loopUntilSaturday() =
                while (DateTime.Now.DayOfWeek <> DayOfWeek.Saturday) do
                    printf "Still working\n"

                printf "Saturday at last!\n"

        module more_iteration_loops_over_sequences =

            let loop_sequence () =
                for (b, pj) in [("Banana 1", false); ("Banana 2", true)] do
                    if pj then
                        printfn "%s is in pyjamas today!" b;

            open System.Text.RegularExpressions
            let loop_regex () =
                for m in Regex.Matches("All the Pretty Horses", "[a-zA-Z]+") do
                    printf "res = %s\n" m.Value

        let run () =
            simple_for_loops.repeatFetch "www.google.com" 2
            //simple_while_loops.loopUntilSaturday()
            more_iteration_loops_over_sequences.loop_sequence()
            more_iteration_loops_over_sequences.loop_regex()
            ()

    module using_mutable_records =

        type DiscreteEventCounter = 
            {   mutable Total : int;
                mutable Positive : int;
                Name : string   }

        let recordEvent (s : DiscreteEventCounter) isPositive =
            s.Total <- s.Total + 1
            if isPositive then s.Positive <- s.Positive + 1

        let reportStatus (s : DiscreteEventCounter) =
            printfn "We have %d %s out of %d" s.Positive s.Name s.Total

        let newCounter nm =
            {   Total = 0;
                Positive = 0;
                Name = nm   }

        let longPageCounter = newCounter "long page(s)"

        let fetch url = 

            let page = http_stuff.http url
            recordEvent longPageCounter (page.Length > 10000)
            page

        let run () =

            fetch "http://www.smh.com.au" |> ignore
            fetch "http://www.theage.com.au" |> ignore
            reportStatus longPageCounter

    module mutable_reference_cells_a =

        let run () =
            let cell1 = ref 1
            printfn "%d" cell1.Value
            // cell1 := 3   // cell must be mutable
            printfn "%d" cell1.Value

            let mutable cell2 = ref 1
            printfn "%d" cell2.Value
            cell2 := 3
            printfn "%d" cell2.Value

    module mutable_reference_cells_b =

        type 'T ref =
            {   mutable contents : 'T   }
            member cell.Value = cell.contents
        let (!) r = r.contents
        let (:=) r v = r.contents <- v
        let ref v = {   contents = v    }

        let run () = ()

    module avoiding_aliasing =

        let run () =
            let mutable cell1 = ref 1
            printfn "%d" cell1.Value
            cell1 := 3
            printfn "%d" cell1.Value

            let mutable cell2 = cell1
            printfn "%d" cell2.Value
            cell2 := 7

            printfn "%d" cell1.Value
            printfn "%d" cell2.Value

    module hiding_mutable_data =

        let generateStamp =
            let count =  ref 0
            (fun () -> count := !count + 1; count)

        let run () =
            let mutable gs = generateStamp()
            printfn "%A" gs
            let mutable gs = generateStamp()
            printfn "%A" gs
            printfn "%A" gs

            printfn "%A" (generateStamp ())
            printfn "%A" (generateStamp ())

    module using_mutable_locals =

        let sum n m =
            let mutable res = 0
            for i = n to m do
                res <- res + i
            res

        let run () =
            printfn "%d" (sum 3 6)

    module working_with_arrays =

        let intro () = 
            let arr = [|1.0; 1.0; 1.0|]
            printfn "%f" arr.[0]
            printfn "%f" arr.[1]
            printfn "%f" arr.[2]
            // printfn "%f" arr.[3]    // System.IndexOutOfRangeException: Index was outside the bounds of the array.
            arr.[1] <- 3.0
            printfn "%A" arr

        let generating_and_slicing_arrays () = 
            let arr = [|for i in 0 .. 5 -> (i, i * i)|]
            printfn "%A" arr
            printfn "%A" arr.[1..3]
            printfn "%A" arr.[..2]
            printfn "%A" arr.[3..]

        let run () =
            intro()
            generating_and_slicing_arrays()

    module introducing_imperative_dot_net_collections =
        module using_resizeable_arrays =

            let example_1 () =
                let names = new ResizeArray<string>()

                for name in ["Claire"; "Sophie"; "Jane"] do
                    names.Add(name)

                printfn "%d" names.Count
                printfn "%s" names.[0]
                printfn "%s" names.[1]
                printfn "%s" names.[2]

            let example_2 () =
                let squares = new ResizeArray<int>(seq {for i in 0 .. 100 -> i * i})

                for x in squares do
                    printfn "square: %d" x

            let run () =
                example_1()
                example_2()

        module using_dictionaries =
            open System.Collections.Generic

            let run () =
                let capitals = new Dictionary<string, string>(HashIdentity.Structural)
                capitals.["USA"] <- "Washington"
                capitals.["Bangladesh"] <- "Dhaka"
                printfn "%A" (capitals.ContainsKey("USA"))
                printfn "%A" (capitals.ContainsKey("Australia"))

                for kvp in capitals do
                    printf "%s has capital %s\n" kvp.Key kvp.Value

        module using_dictionaries_try_get_value =
            open System.Collections.Generic

            let lookUpName nm (dict : Dictionary<string, string>) =
                let mutable res = ""
                let foundIt = dict.TryGetValue(nm,&res)
                if foundIt then res
                else failwithf "Diidn't find %s" nm

            let run () =
                let capitals = new Dictionary<string, string>(HashIdentity.Structural)
                capitals.["USA"] <- "Washington"
                capitals.["Bangladesh"] <- "Dhaka"

                printfn "%A" (lookUpName "USA" capitals)
                printfn "%A" (lookUpName "Bangladesh" capitals)
                // *********** THE FOLLOWING WILL THROW EXCEPTIONS
                //printfn "%A" (lookUpName "India" capitals)
                //printfn "%A" (lookUpName "Germany" capitals)


        module using_dictionaries_with_compound_keys =
            open System.Collections.Generic

            let run () =
                let sparseMap = new Dictionary<(int * int), float>()
                sparseMap.[(0, 2)] <- 4.0
                sparseMap.[(1021, 1847)] <- 9.0
                printfn "%A" sparseMap.Keys

        let run () =
            using_resizeable_arrays.run()
            using_dictionaries.run()
            using_dictionaries_try_get_value.run()
            using_dictionaries_with_compound_keys.run()

    module exceptions_and_controlling_them =
        module catching_exceptions =
            let simple_try_catch () =
                try
                    raise (System.InvalidOperationException("not today thank you"))
                with
                    :? System.InvalidOperationException -> printfn "caught!"
                
            // This is not in the book. This was generated by Visual Studio Copilot.
            let http_with_exceptions (url : string) =
                try
                    // Use HttpClient instead of the obsolete WebRequest APIs.
                    // Return an option<string> so the try/with is an expression (fixes FS0588).
                    use client = new System.Net.Http.HttpClient()
                    let task = client.GetStringAsync(url)
                    Some (task.Result)
                with
                | :? System.UriFormatException ->
                    printfn "Invalid URL: %s" url
                    None
                | :? System.Net.Http.HttpRequestException as ex ->
                    printfn "Request failed: %s" ex.Message
                    None
                | ex ->
                    printfn "Unexpected error: %s" ex.Message
                    None

            let run () =
                simple_try_catch()
                http_with_exceptions("giberrish")   // throws exception                

        let run () =
            catching_exceptions.run()
            
    module having_an_effect_basic_io =
        open System.IO

        let intro () =
            // create a file and write lines directly into the file 
            File.WriteAllLines("test.txt", 
                            [|"This is a test.";
                                       "It is easy to read"|])

            // open a file and read all the lines directly from the file 
            File.ReadAllLines "test.txt"

        let read_sequence () =
            // create a file and write lines directly into the file 
            File.WriteAllLines("test.txt", 
                            [|"This is a test.";
                                       "It is easy to read"|])

            // create a sequence from the file and yield a specific line
            seq {
                for line in File.ReadLines("test.txt") do
                    let words = line.Split [|' '|]
                    if words.Length > 3 && words.[2] = "easy" then
                        yield line
            }

        let run () =
            intro() |> printfn "Contents of File: %A"
            read_sequence() |> printfn "Sequence from a File: %A"

    module dotnet_io_via_streams =
        open System.IO

        let run () =
            let outp = File.CreateText "playlist.txt"
            outp.WriteLine "Enchanted"
            outp.WriteLine "Put your records on"
            outp.Close()

            let inp = File.OpenText "playlist.txt"
            inp.ReadLine() |> printfn "%A"
            inp.ReadLine() |> printfn "%A"
            inp.Close()

            System.Console.WriteLine "Hello World"
            System.Console.ReadLine() |> ignore

    module precomputation_and_partial_application =
        open System.Collections.Generic // for HashSet

        // precompute search lists of words
        let isWord (words : string list) = 
            let wordTable = Set.ofList words
            fun w -> wordTable.Contains(w)  // returns new function

        // create lookup function isCapital by precomputing capital
        // table with partial application of lists of capitals to isWord
        let isCapital =
            isWord ["London"; "Paris"; "Warsaw"; "Tokyo"]

        // slow implementation of isCapital.
        // for every invocation of isCapitalSlow the look up table
        // is evaluated
        let isCapitalSlow inp =
            isWord ["London"; "Paris"; "Warsaw"; "Tokyo"] inp

        // the following versions are equally slower
        let isWordSlow2 (words : string list) (word : string) =
            List.exists (fun word2 -> word = word2) words

        let isCapitalSlow2 word =
            isWordSlow2 ["London"; "Paris"; "Warsaw"; "Tokyo"] word

        // use a HashSet ineffectively: HashSet is constructed at every invocation
        let isWordSlow3 (words : string list) (word : string) =
            let wordTable = Set<string>(words)
            wordTable.Contains(word)

        let isCapitalSlow3 word =
            isWordSlow3 ["London"; "Paris"; "Warsaw"; "Tokyo"] word

        // the most efficient vesrion
        module efficient =            
            // use a HashSet effectively. HashSet is constructed at first invocation
            // of isWord. isWord returns a function so we have a closure on wordTable
            let isWord (words : string list) =
                let wordTable = HashSet<string>(words)
                fun word -> wordTable.Contains word

            // isCapital initialises the closure on isWord
            let isCapital =
                isWord ["London"; "Paris"; "Warsaw"; "Tokyo"]

        let run () =
            isCapital "Paris" |> printfn "%A"
            isCapital "Manchester" |> printfn "%A"
            isCapitalSlow "Paris" |> printfn "%A"
            isCapitalSlow "Manchester" |> printfn "%A"
            isCapitalSlow2 "Paris" |> printfn "%A"
            isCapitalSlow2 "Manchester" |> printfn "%A"
            efficient.isCapital "Paris" |> printfn "%A"
            efficient.isCapital "Manchester" |> printfn "%A"

    module precomputation_and_objects =
        open System.Collections.Generic // for HashSet

        type NameLookupService =
            abstract Contains : string -> bool

        // Returns constructued type NameLookupService
        let buildSimpleNameLookup (words : string list) =
            let wordTable = HashSet<_>(words)
            {new NameLookupService with
                 member t.Contains w = wordTable.Contains w}

        let capitalLookup = buildSimpleNameLookup ["London"; "Paris"; "Warsaw"; "Tokyo"]
        let isCapital word = capitalLookup.Contains word

        let run () =
            isCapital "Paris" |> printfn "%b"
            isCapital "Manchester" |> printfn "%b"

    module memoizing_computations =
        let rec fib n = if n <= 2 then 1 else fib(n - 1) + fib(n - 2)
        let fibFast =
            let t = new System.Collections.Generic.Dictionary<int, int>()
            let rec fibCached n =
                if t.ContainsKey n then t.[n]
                elif n <= 2 then 1
                else let res = fibCached(n - 1) + fibCached(n - 2)
                     t.Add (n, res)
                     res
            fun n -> fibCached n

        let run () =
            time_stuff.time(fun () -> fibFast 30) |> printfn "%A"
            time_stuff.time(fun () -> fibFast 30) |> printfn "%A"
            time_stuff.time(fun () -> fibFast 30) |> printfn "%A"
            time_stuff.time(fun () -> fib 30)  |> printfn "%A" 

    module generic_memoizing_function =
        open System.Collections.Generic // for HashSet
        let memoize (f : 'T -> 'U) =
            let t = new Dictionary<'T, 'U>(HashIdentity.Structural)
            fun n ->
                if t.ContainsKey n then t.[n]
                else let res = f n
                     t.Add(n, res)
                     res

        #nowarn "40"    // do not warn on recursive computed objects and function
        let rec fibFast = 
            memoize (fun n -> if n <= 2 then 1 else fibFast(n - 1) + fibFast(n - 2))

        // THIS IS A VERY BAD USE OF A LOOK-UP TABLE.
        // EACH INVOCATION OF fibNotFast CREATES A 
        // NEWS EMPTY INSTANCE OF THE LOOK-UP TABLE.
        let rec fibNotFast n = 
            memoize (fun n -> if n <= 2 then 1 else fibNotFast(n - 1) + fibNotFast(n - 2)) n

        let run () =
            time_stuff.time(fun () -> fibFast 30) |> printfn "%A"
            time_stuff.time(fun () -> fibFast 30) |> printfn "%A"
            time_stuff.time(fun () -> fibFast 30) |> printfn "%A"
            time_stuff.time(fun () -> fibNotFast 30)  |> printfn "%A"
            time_stuff.time(fun () -> fibNotFast 30)  |> printfn "%A" 
            time_stuff.time(fun () -> fibNotFast 30)  |> printfn "%A" 

    module generic_memoization_service =
        open System.Collections.Generic // for Dictionary, HashSet

        type Table<'T, 'U> =
            abstract Item : 'T -> 'U with get
            abstract Discard : unit -> unit

        let memoizeAndPermitDiscard f =
            let lookasideTable = new Dictionary<_,_>(HashIdentity.Structural)
            {new Table<'T,'U> with
                member T.Item
                   with get (n) =
                        if lookasideTable.ContainsKey(n)
                        then lookasideTable.[n]
                        else let res = f n
                             lookasideTable.Add(n, res)
                             res

                member t.Discard (): unit = 
                   lookasideTable.Clear()}

        #nowarn "40"    // do not warn on recursive computed objects and function
        let rec fibFast = 
            memoizeAndPermitDiscard
                (fun n ->
                        printfn "computing fibFast %d"  n
                        if n <= 2 then 1 else fibFast.[n - 1] + fibFast.[n - 2])

        let run () =
            time_stuff.time(fun () -> fibFast.[3]) |> printfn "%A"
            time_stuff.time(fun () -> fibFast.[5]) |> printfn "%A"
            fibFast.Discard()
            time_stuff.time(fun () -> fibFast.[5]) |> printfn "%A"
            time_stuff.time(fun () -> fibFast.[30]) |> printfn "%A"
            time_stuff.time(fun () -> fibFast.[31]) |> printfn "%A"

    module lazy_values = 
        let intro () =
            let sixty = lazy (30 + 30)
            printfn "%d", sixty.Force()

            let sixtyWithSideEffects = lazy (printfn "Hello World", 30 + 30)
            printfn "%d", sixtyWithSideEffects.Force()

        let run () =
            intro()

    module replace_mutable_locals_loops_with_recursion = 
        let factorizeImperative n =
            let mutable factor1 = 1
            let mutable factor2 = n
            let mutable i = 2
            let mutable fin = false
            while (i < n && not fin) do
                if (n % i = 0) then
                    factor1 <- i
                    factor2 <- n / i
                    fin <- true
                i <- i + 1

            if (factor1 = 1) then None
            else Some(factor1, factor2)

        let factorizeRecursive n=
            let rec find i =
                if i >= n then None
                elif (n % i = 0) then Some(i, n / i)
                else find(i + 1)
            find 2

        let run () =
            factorizeImperative(20) |> printfn "%A"
            factorizeRecursive(20) |> printfn "%A"

    module separating_mutable_data_structures =
        open System.Collections.Generic // for Dictionary, HashSet

        let divideIntoEquivalentClasses keyf seq = 
            // The dictionary to hold the equivalent classes
            let dict = new Dictionary<'key, ResizeArray<'T>>()
            // Build the groupings
            seq |> Seq.iter (fun v ->
                let key = keyf v
                let mutable prev = Unchecked.defaultof<ResizeArray<'T>>
                if dict.TryGetValue(key, &prev) then
                    prev.Add(v)
                else
                    let newPrev = ResizeArray<'T>()
                    newPrev.Add(v)
                    dict.[key] <- newPrev)

            // Return the sequence-of-sequences. Don't reveal the
            // internal collections: just reveal them as sequences
            dict |> Seq.map (fun group ->
                                            group.Key, Seq.readonly group.Value)

        let run () =
            divideIntoEquivalentClasses (fun n -> n % 2) [0 .. 10] |> printfn "%A"
            divideIntoEquivalentClasses (fun n -> n % 3) [0 .. 10] |> printfn "%A"

    module avoid_combining_imperative_programming_and_laziness =
        open System.IO

        // Example 1:
        let wrong () =
            let reader1, reader2 =
                let reader = new StreamReader(File.OpenRead("test.txt"))
                let firstReader() = reader.ReadLine()       // note: reader is now in a function body
                let secondReader() = reader.ReadLine()      // note: reader is now in another function body

                // Note: we close the stream reader here!
                // But we are returning functions which use the closed reader
                // This is very bad!
                reader.Close()
                firstReader, secondReader

            // Note: stream reader is now closed! The next line will fail
            let firstLine = reader1()
            let secondLine = reader2()
            firstLine, secondLine

        // Example 2:
        // Store values from disposable resource
        let correct_1 () =
            let line1, line2 =
                let reader = new StreamReader(File.OpenRead("test.txt"))
                let firstLine = reader.ReadLine()       // note: reader assigns value read
                let secondLine = reader.ReadLine()      // note: reader assigns another value read

                // Note: we close the stream reader here!
                // We are returning lines already assigned from the reader
                // This is alright
                reader.Close()
                firstLine, secondLine

            // Note: stream reader is now closed! But we can still copy or reference the values
            let firstLine = line1
            let secondLine = line2
            firstLine, secondLine

        // Example 3:
        // Embed the StreamReader in a longer life time object
        // like a sequence before it is returned.
        let correct_2 () =
            let lineOfFile =
                seq {
                    use reader = new StreamReader(File.OpenRead("test.txt"))
                    while not reader.EndOfStream do
                        yield reader.ReadLine()
                }
            lineOfFile

        let run () =  ()

    module execute_modules =
        let run () =
            printfn "[---- Expert F#: START CHAPTER 4 ----]"

            imperative_looping_and_iterating.run()
            using_mutable_records.run()
            mutable_reference_cells_a.run()
            mutable_reference_cells_b.run()
            avoiding_aliasing.run()
            hiding_mutable_data.run()
            using_mutable_locals.run()
            working_with_arrays.run()
            introducing_imperative_dot_net_collections.run()
            exceptions_and_controlling_them.run() |> ignore
            having_an_effect_basic_io.run()
            dotnet_io_via_streams.run()
            precomputation_and_partial_application.run()
            precomputation_and_objects.run()
            memoizing_computations.run()
            generic_memoizing_function.run()
            generic_memoization_service.run()
            lazy_values.run() |> ignore
            replace_mutable_locals_loops_with_recursion.run()
            separating_mutable_data_structures.run()

            printfn "[---- Expert F#: END CHAPTER 4 ----]"
            printfn ""


