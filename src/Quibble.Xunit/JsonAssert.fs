﻿namespace Quibble.Xunit

open Xunit.Sdk
open Quibble    

type JsonAssertException(expected: obj, actual: obj, messages: string list) =
    inherit AssertActualExpectedException(expected, actual, Message.toUserMessage messages)
    
    member self.DiffMessages = messages

type JsonDiffConfig = {
    allowAdditionalProperties: bool;
    ignoreValueDiffAtPath: string
}

module JsonAssert =
    
    let Equal (expectedJsonString: string, actualJsonString: string) : unit =
        let diffs : Diff list = JsonStrings.diff actualJsonString expectedJsonString
        let messages : string list = diffs |> List.map Message.toAssertMessage
        if not (List.isEmpty messages) then
            let ex = JsonAssertException(expectedJsonString, actualJsonString, messages)
            raise ex
        
    let EqualOverrideDefault (expectedJsonString: string, actualJsonString: string, diffConfig : JsonDiffConfig) : unit =
        let checkDiffConfig (diffConfig : JsonDiffConfig) (diff : Diff) =
            match diff with
            | ValueDiff valueDiff ->
                if diffConfig.ignoreValueDiffAtPath = valueDiff.Path then None else Some diff
            | ObjectDiff (diffPoint, mismatches) ->
                if diffConfig.allowAdditionalProperties then
                    // Left JSON <-> Actual JSON (Left-only properties are additional properties.)
                    // Right JSON <-> Expected JSON (Right-only properties are missing properties.)
                    let keepRightOnly =
                        function
                        | LeftOnlyProperty _ -> false
                        | RightOnlyProperty _ -> true
                    let remaining = mismatches |> List.filter keepRightOnly
                    match remaining with
                    | [] -> None
                    | _ -> Some <| ObjectDiff (diffPoint, remaining)
                else Some diff
            | _ -> Some diff
        let diffs : Diff list = JsonStrings.diff actualJsonString expectedJsonString
        let diffs' = diffs |> List.choose (checkDiffConfig diffConfig)
        let messages : string list = diffs' |> List.map Message.toAssertMessage
        if not (List.isEmpty messages) then
            let ex = JsonAssertException(expectedJsonString, actualJsonString, messages)
            raise ex
            