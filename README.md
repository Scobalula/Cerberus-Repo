# Cerberus
[![Releases](https://img.shields.io/github/downloads/Scobalula/Cerberus-Repo/total.svg)](https://github.com/Scobalula/Greyhound/releases) [![Discord](https://img.shields.io/badge/chat-Discord-blue.svg)](https://discord.gg/fGVpV39)

Cerberus is a GSC/CSC Decompiler for Call of Duty: Black Ops II/III. It's main purpose is to provide access to scripts that Treyarch did not provide in the Call of Duty: Black Ops III Mod Tools and to give greater insight into how Treyarch achieved certain things, undocumented APIs, etc.

# Features:
* Easy to use, simply load the UI and drag/load scripts or use the CLI version for batch
* Smart loop detection, Cerberus will check for common patterns for foreach and check variable references for for loops
* Fast, it can decompile Black Ops III's ZM script almost instantly
* Built-in Fast File Extractor for Black Ops III\*
* Both a CLI and UI version

\* This is only supported in the CLI program, not the UI.

# Requirements

* Windows 7 x86 and above
* .NET Framework 4.7.2
* Microsoft Visual C++ 2019 Runtime

# Using Cerberus

The latest version can be downloaded from the [Released](https://github.com/Scobalula/Cerberus-Repo/releases) section.

By using Cerberus you are agreeing to the license terms included in the download and on the [License](https://github.com/Scobalula/Cerberus-Repo/releases) page.

Once downloaded you can either use the UI or CLI version and drag and drop/load scripts. In the UI version you can double click scripts to load them and click functions to jump to them in the decompiler/disassembly. Check the "HowToUse.txt" in the download for more info on UI shortcuts, etc.

# Credits:

* kokole and Nukem - Basics of how the VM works from their original gsc reader

# Limitations:

As with any decompiler there are limitions to the output Cerberus provides.

* Some for loops may not be properly marked, or detected, but this is rare.

* Custom GSCs compiled using Black Ops III's Mod Tools are not supported. This is a decision that will remain forever as we all know how that would end up.

* Starting with Black Ops III they've started hashing almost everything, including notifies, etc. so some data will output as var_\<hash\>, etc. if it's not found in the name database.

* Black Ops III also introduced "debug strings". Any strings within dev blocks will output as "Dev block strings are not support".

* Currently operators and compound assigments are dumped as the decompiler receives them, so check the disassembly to ensure order of operations is correct. This will be fixed in a future update.

* Ternary operators are currently not supported, you can identify them with an empty if/else, you can see what it would have been by looking at the disassembly. They will be supported in a future update.

* Classes for Black Ops III cannot currently be reconstructed and their output is crude at best. It should still give some insight, but keep this in mind as they are essentially glorified structs with some object orientated features. Unusued properties are scrapped among other things.

* The decompiler can some times incorrectly mark blocks as childrent as other blocks, this is more an issue with cases due to being different, but it's very rare. Please file an issue either on Discord or here with the name of the script and fast file.

* Not all op codes are supported for Black Ops III, if you hit an unknown op code DO NOT file an issue, I am aware of it. Both the decompiler and disassembler will just skip that op code/function.

# FAQ

* **I found a genuine issue with Cerberus and/or its output, how can I report it?**
* Thanks for being the 10% that actually wants to legitimate report an issue instead of making claims, etc. about my tools in groups I'm not even apart of. You can file an issue either on Github or via my Discord server, pinging me on Twitter works too.

* **How come some stuff is named var_49dbff, etc. but some stuff is named?**

* As mentioned above Black Ops III hashes everything, a table has been built with all source gsc files I could find from CoD 4, 5, Bo1, and Bo3. Some stuff is not apart of this table.

* **I compared the output from Cerberus to a script we have and some of it is different, etc?**

* A decompiler will never reproduce the exact source code, but the control flow should match. An understanding of how a compiler works is a must, your code is parsed and instructions are generated matching the control, etc. With this in mind there are many ways to achieve the same thing, and so Cerberus will produce what it thinks is a good representation of the instructions.

* **I tried compiling the decompiled output and it didn't work?**
* Very rarely will a decompiler produce 100% decompilable output. The output is to be studied, not copy and pasted.

* **I noticed you said custom scripts are not supported, but I really need access to my old scripts, is there any way to get them?**

* I can take requests to access scripts from a map you own and need access to, but you need to make a good case to me as I'm not spending my time dumping people's maps.

* **uR aPP is crAP**
* Sorry to hear that, this is the first decompiler I've ever written so it's a big learning experience for me.

# Disclaimers

Cerberus is currently in a very early Alpha state but should be mostly functional. With this in mind there will be issues with Cerberus itself and the output it produces.

Cerberus was developed for the users of the Black Ops III Mod Tools to provide some files/information Treyarch couldn't/didn't include with the Mod Tools. All work was done on legally obtained copies of Black Ops III and the Black Ops III Mod Tools. Most of the files it exports, are only useful to those using Black Ops III's Mod Tools. I don't aid in killing other players through walls!

Cerberus is provided AS IS, do not ask how to do XYZ, etc. You need to know what you're doing, we can't hold your hand through it.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*This tool is currently not open source and this repo just serves as documentation, issue, and release tracking. It will be open sourced down the line once I have fully completed it*