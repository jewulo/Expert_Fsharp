module chapter_2

open System.IO
open System.Net.Http
open System.Threading.Tasks

// I had to add these lines to the project XML to get System.Windows.Forms
// <TargetFramework>net6.0-windows</TargetFramework>
// <UseWindowsForms>true</UseWindowsForms>
open System.Windows.Forms

// For more information see https://aka.ms/fsharp-console-apps

let run() = 
    /// Split a string into words at spaces
    let splitAtSpaces (text: string) =
        text.Split ' '
        |> Array.toList

    /// Analyse a string for duplicate words
    let wordCount text =
        let words = splitAtSpaces text
        let wordSet = Set.ofList words
        let numWords = words.Length
        let numDups = words.Length - wordSet.Count
        (numWords, numDups)

    /// Analyse a string for duplicate words and display the results
    let showWordCount text =
        let numWords, numDups = wordCount text
        printfn "--> %d words in the text" numWords
        printfn "--> %d duplicate words" numDups

    let (numWords, numDups) = wordCount "All the king's horses and all the kings's men"
    showWordCount "Couldn't put Humpty together again"

    let powerOfFour n =
        let nSquared = n * n
            in nSquared * nSquared

    let res = powerOfFour 3
    printfn "%d" res

    /// bindings
    let powerOfFourPlusTwo_1 n =
        let n = n * n
        let n = n * n
        let n = n + 2
        n

    /// .... is equivalent to
    let powerOfFourPlusTwo_2 n =
        let n1 = n * n
        let n2 = n1 * n1
        let n3 = n2 + 2
        n3

    let powerOfFourPlusTwoTimesSix n =
        let n3 =
            let n1 = n * n
            let n2 = n1 * n1
            n2 + 2
        let n4 = n3 * 6
        n4

    let sol1 = Set.ofList ["b"; "a"; "b"; "b"; "c"]
    let sol2 = Set.ofList ["abc"; "ABC"]
    let sol3 = Set.toList (Set.ofList ["abc"; "ABC"])

    /// a generic function defuined using member function of generic type list<T>
    let length (inp:'T list) = inp.Length
    /// inp.Length is not resolvable due to a lack of enough type information
    //let length2 inp = inp.Length

    /// Tuples
    let site1 = ("www.cnn.com", 10)
    printfn "%A" site1
    printfn "%A" (fst site1)
    printfn "%A" (snd site1)

    let site2 = ("www.bbc.com", 5)
    printfn "%A" site2
    printfn "%A" (fst site2)
    printfn "%A" (snd site2)

    let site3 = ("www.msnbc.com", 4)
    printfn "%A" site3
    printfn "%A" (fst site3)
    printfn "%A" (snd site3)

    let sites = (site1, site2, site3)
    printfn "%A" sites

    // decomposing a tuple
    let url, relevance = site1
    printfn "%A" url
    printfn "%A" relevance

    let siteA, siteB, siteC = sites
    printfn "%A" siteA
    printfn "%A" siteB
    printfn "%A" siteC

    /// see [wordCount text: string -> int * int] above
    let showResults (numWords, numDups) = 
        printfn "--> %d words in text" numWords
        printfn "--> %d duplicate words" numDups

    let showWordCount2 text = showResults (wordCount text)

    printfn "%A" (showWordCount2 "All the king's horses and all the kings's men")
    printfn "%A" (showWordCount2 "Couldn't put Humpty together again")

    System.Console.WriteLine("--> {0} words in the text", box numWords)
    System.Console.WriteLine("--> {0} duplicate words", box numDups)

    /// separinting sequencial code with a semicolon;
    /// the value is the last expression in the sequence
    let two = (printfn "Hello World"; 1 + 1)
    let four = two + two
    printfn "%A" two
    printfn "%A" four

    (printfn "--> %d words in text" numWords;
     printfn "--> %d duplicate words" numDups)

    (*
    ///
    /// THIS SECTION DOES NOT COMPILE ///
    ///

    open System.Windows.Forms

    let form = new Form(Visible = true, TopMost = true, Text = "Welcome to F#")

    let textB = new RichTextBox(Dock = DockStyle.Fill, Text = "Here is some text")
    form.Control.Add textB
    *)

    (*
    open System.IO
    open System.Net


    /// Get the contents of ther URL via a web request
    let http (url: string) =
        let req = System.Net.WebRequest.Create(url)
        let resp = req.GetResponse()
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let html = reader.ReadToEnd()
        resp.Close()
        html

    textB.text <- http "http://news.bbc.co.uk"
    *)

    // I had to add these lines to the project XML to get System.Windows.Forms
    // <TargetFramework>net6.0-windows</TargetFramework>
    // <UseWindowsForms>true</UseWindowsForms>
    

    let form0 = new System.Windows.Forms.Form(Visible = true, TopMost = true, Text = "Welcome to F#")
    let textB = new System.Windows.Forms.RichTextBox(Dock = DockStyle.Fill, Text = "Here is some initial text") 
    form0.Controls.Add textB

    /// Get the contents of the URL via a web request using HttpClient (async)
    let http (url: string) =
        use client = new HttpClient()
        let task: Task<string> = client.GetStringAsync(url)
        task.Result

    textB.Text <- http "http://news.bbc.co.uk"
    printfn "%A" (http "http://news.bbc.co.uk")

    let form1 = new System.Windows.Forms.Form()
    form1.Visible <- true
    form1.TopMost <- true
    form1.Text <- "Welcome to F#"

    let form2 = form1
    form2.Text <- "F# Forms are Fun"

    let textC = new RichTextBox(Dock = DockStyle.Fill)
    form1.Controls.Add(textB)
    ()