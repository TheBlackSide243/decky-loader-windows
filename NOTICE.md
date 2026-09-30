# Third-party notice

The MIT licence in [LICENSE](LICENSE) covers Decky Manager only.

Decky Loader (https://github.com/SteamDeckHomebrew/decky-loader) is a separate
project licensed under the GNU General Public License v2.0. No part of its code
is included in this repository, and released builds of Decky Manager do not
bundle its binaries: they are downloaded from the Decky Loader CI at install
time. A build produced with `build.ps1 -Embed` does bundle them, and
redistributing such a build carries the obligations of the GPL-2.0.
