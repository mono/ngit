ABOUT NGIT
----------

NGit is a port of JGit [1] to C#. This port is generated semi-automatically
using Sharpen [2], a Java-to-C# conversion utility.

NGit provides all functionality implemented by JGit, including all repository
manipulation primitives and transport protocols. SSH support is provided by
a port of jsch [3], included in the project.

The project is composed by 4 libraries:
- NGit: The git library.
- NGit.Test: Unit tests for NGit
- NSch: The port of jsch.
- Sharpen: Some support classes required by the above libraries.

The code included in this project is already converted, so to use it
you just have to open the ngit.sln solution and build it.

Instructions and tools for updating and regenerating the NGit code from JGit
are available in the 'gen' subdirectory.

COMPILING
---------
The port depends on two external libraries:
  - ICSharpCode.SharpZipLib
  - Mono.Security
  - Mono.Posix (optional)

If you are compiling with Mono then these libraries will be available in
Mono's GAC. If you are compiling on Windows using the Microsoft .NET
framework you can obtain these libraries by installing the Mono Libraries
package:
  http://monodevelop.com/files/Windows/MonoLibraries.msi

The optional Mono.Posix assembly can be gotten by installing Gtk# for windows.
The latest installer can usually be found on the monodevelop site:
  http://monodevelop.com/Download

Mono.Posix is only required when building the Sharpen.Unix assembly, and this
assembly is only required when running NGit on MacOS or Linux operating system.
If you are only running on Windows, then you do not need to compile this assembly.
Sharpen.Unix only contains support code to correctly handle symlinks on Unix
based systems.

CREDITS
-------

Credits on the code should go to the authors of jgit, jsch and Sharpen
(see links below).

The support Sharpen library has been implemented by Lluis Sanchez (lluis@novell.com)

[1] http://eclipse.org/jgit
[2] http://developer.db4o.com/Projects/html/projectspaces/db4o_product_design/sharpen.html
[3] http://www.jcraft.com/jsch
