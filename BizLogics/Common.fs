﻿module BizLogics.Common

open System
open System.Text
open System.Collections.Generic
open System.Collections.Concurrent

open Util.Text
open Util.Crypto
open Util.DbTx
open Util.Zmq

open UtilWebServer.DbLogger
open UtilWebServer.Db
open UtilWebServer.Common

open Shared.OrmTypes
open Shared.Types
open Shared.OrmMor

type Host = {
zmq: bool
port: int
conn: string
defaultHtml: string
fsDir: string }

type EuComplex = {
eu: EU }

type BizComplex = {
biz: BIZ }

type SessionRole =
| EndUser of EuComplex
| Visitor

type Session = UtilWebServer.Auth.Session<SessionRole,unit>

type Runtime = {
host: Host
ecs: ConcurrentDictionary<int64,EuComplex>
bcs: ConcurrentDictionary<string,BizComplex>
bizowners: ConcurrentDictionary<int64,BIZOWNER>
sessions: ConcurrentDictionary<string,Session>
output: string -> unit }

type HostEnum = 
| Prod
| RevengeDev

let hostEnum = 
    match Environment.MachineName with
    | "MAIN" -> RevengeDev
    | _ -> Prod

let host e = 

    match e with
    | Prod -> 
        {
            zmq = true
            port = 80
            conn = "server=.; database=GCHAIN; Trusted_Connection=True;"
            defaultHtml = "index.html"
            fsDir = @"C:\Dev\GCHAIN2024\GChainVsOpen\Deploy" }
    | RevengeDev -> 
        {
            zmq = false
            port = 80
            conn = "server=.; database=GCHAIN; Trusted_Connection=True;"
            defaultHtml = "index.html"
            fsDir = @"C:\Dev\GCHAIN2024\GChainVsOpen\Deploy" }

let runtime = 
    let host = host hostEnum

    {
        host = host
        ecs = new ConcurrentDictionary<int64,EuComplex>()
        bcs = new ConcurrentDictionary<string,BizComplex>()
        bizowners = new ConcurrentDictionary<int64,BIZOWNER>()
        sessions = new ConcurrentDictionary<string,Session>()
        output = output }
