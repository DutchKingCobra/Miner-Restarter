# Miner-Restarter
Restart your miner at intervals, Configurable for all miners like Phoenix / trex / nbminer etc etc

I needed to restart Phoenixminer every so often so i wrote this
if miner is not running start it and if its running for a certain amount of time restart it.

Config file is generated with the following settings:
```
[Settings]
Minerdirectory=c:\phoenixminer\
MinerExecName=phoenixminer
RestartIntervalHours=48
CheckMinerCrashIntervalSec=10
CommandLine=-epool etc-eu1.nanopool.org:19999 -ewal 0xe8ac5095915d9c6367daa3d0d2bc146f592fd935.Rig02 -pass x 
```
