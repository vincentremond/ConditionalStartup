open System.Diagnostics
open System.Text
open NativeWifi
open Pinicola.FSharp.SpectreConsole

[<RequireQualifiedAccess>]
module Dot11Ssid =
    let toString (ssid: Wlan.Dot11Ssid) : string =
        Encoding.UTF8.GetChars(ssid.SSID)
        |> Array.take (int ssid.SSIDLength)
        |> System.String

[<RequireQualifiedAccess>]
module Process =
    let fireAndForget fileName workingDirectory (arguments: string seq) =
        let processStartInfo =
            ProcessStartInfo(fileName, arguments, UseShellExecute = false, WorkingDirectory = workingDirectory)

        let process_ = new Process(StartInfo = processStartInfo)
        let started = process_.Start()
        if not started then
            failwithf $"Failed to start process {fileName} with arguments {arguments}"
        

let getCurrentWifiSsid () =
    let wlanClient = WlanClient()

    let connectedInterfaces =
        wlanClient.Interfaces
        |> Array.filter (fun i -> i.InterfaceState = Wlan.WlanInterfaceState.Connected)

    match connectedInterfaces with
    | [| connectedInterface |] ->
        connectedInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid
        |> Dot11Ssid.toString
    | [||] -> failwith "No connected interfaces found"
    | _ -> failwithf $"Multiple connected interfaces found ({connectedInterfaces})"

let currentSsid = getCurrentWifiSsid ()

printfn $"Current SSID: {currentSsid}"

match currentSsid with
| @"Livebox-C390" ->
    AnsiConsole.markupLineInterpolated $"Connected to [green]{currentSsid}[/]."

    AnsiConsole.markupLine "Starting [green]Google Chrome[/] with [green]Profile 2[/]."

    Process.fireAndForget
        "C:\Program Files\Google\Chrome\Application\chrome.exe"
        "C:\Program Files\Google\Chrome\Application"
        [ "--profile-directory=\"Profile 2\"" ]

| other -> AnsiConsole.markupLineInterpolated $"Connected to [yellow]{other}[/]. Nothing to do."
