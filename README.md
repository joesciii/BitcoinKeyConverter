# BitcoinKeyConverter

Converts hex format bitcoin private keys to WIF format safely, offline. Option to dump converted keys to file.
No real error handling so beware, only 64 character hex format bitcoin keys are taken. I wrote it quickly for myself so I could parse hundreds of keys safely offline instead of using a website like https://gobittest.appspot.com/PrivateKey. 

Code is given as experimental.

## Dependencies

*.NET 4.6.1 (Probably bundled with your Windows 10)

## How to use

Download the built version from the latest release here https://github.com/joesciii/BitcoinKeyConverter/releases/tag/v0.12 (or clone and build yourself). Instructions are given within the program. As above, only 64 character hex format private keys are accepted.

## Version History

* 0.12
    * Refactored for potential future functionality. More basic sense checking.
* 0.11
    * Basic input sense check added. Cleaned a bit.
* 0.1
    * Base version

## Acknowledgements

* [CodesInChaos](https://gist.github.com/CodesInChaos/3175971) - Succint Base58 Encoding snippet




