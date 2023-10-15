## Twisted IK 2 documentation landing page

**Thanks for downloading Twisted IK 2! This page shows the basic setup, as well as describing how to use the documentation.**

Other pages contain information relating to other parts of the plugin, as mentioned further below. If you spot an error, something out of date, want to help, or otherwise just have feedback on the documentation, please make a topic on the Itch.IO forums and I will get back to you as soon as I can. Thanks!

**The documentation is still very much a work in progress! Several pages are missing, but as time goes on the documentation will continue to improve.**

### Setup

First, you need to download [Twisted IK 2](https://twistedtwigleg.itch.io/twistedik2) from Itch.IO, if you have not already. If you have bought Twisted IK 2, you can download the paid version, and if not, you can download the free version (you just cannot use it commercially).

Once downloaded, you should have a ZIP folder with the following basic structure:

* License.md
* Addons
  * TwistedIK2
  * TwistedIK2_EditorPlugin

You need to drop all of these files into your Godot project, either a new project or an existing one. Twisted IK 2 uses the C# programming language, so you will need to use a version of Godot that supports C#.

- In other words, the standard GDScript-only version of Godot will not work with this plugin! In the future, there may be a GDScript version of Twisted IK 2, but for now its C# only and that require the C#/Mono version of Godot.

After you have dropped the files into your project, you need to open the project in the Godot editor. Next, you need to open `project_settings` and then select the `plugins` tab. You should see there are two plugins: Twisted IK 2 and Twisted IK 2 Editor Plugin. You need to enable the plugins in this order:

* Twisted IK 2
* Twisted IK 2 Editor Plugin

Once this is done, you should find that you have a bunch of new nodes, all starting with `twisted_`, and that there is a dock on the bottom of the editor called Twisted IK 2.

___________

There is one more step, and then the plugin is fully ready for use! The last step is that you need to build the C# project. There are a couple ways to do this:

* If you already have C# scripts in your project before adding Twisted IK 2, just click the little "build" button in the top right of the Godot editor. This will build all of the C# scripts in your project, including Twisted IK 2.
* If you do not have any C# scripts in your project, you have two options to generate a C# project:
	* The first is to make a new C# script, leave it as the default, and then press the "build" button on the top right of the Godot editor. Once this is done, you can delete the C# script.
	* In the top left bar, select `project`, then `tools`, and finally `mono`. There should be an option to generate a C# project. Select this, and then press "build" in the top right of the screen.

Once the project is built, you should be able to use all of the Twisted IK 2 nodes without any difficulties! If you have any problems, please let me know and I will do my best to get back to you and help!

- **NOTE:** The use of the Twisted IK 2 editor plugin is optional! As of when this was written, the plugin is rather basic and only really helps remove the manual work of making Twisted_Bone3D nodes for a Skeleton, but in the future more functionality will (likely) be added.
	- Also: You will need to **disable the Twisted IK 2 Editor Plugin for HTML5 exports to work!** This is a known limitation of the Twisted IK 2 Editor Plugin. Thanks to [@NHodgesVFX on Twitter](https://twitter.com/NHodgesVFX/status/1340413423797022728) for this discovery!


### How to use the documentation

The documentation is broken out into many separate markdown files, where each file is for a specific topic and/or function. The idea is that you can look in the documentation folder, find the markdown file that covers what you are looking for, and then read it.

For example, if you are looking to learn more about FABRIK, you'll want to look at the `TIK2_doc_FABRIK.md` file. Likewise, if you are wanting to learn more about how to setup your Skeleton for IK use, then `TIK2_doc_SkeletonSetup.md` would be the file you are looking for.

____________

Right now not everything in the plugin has its own documentation file, and because its all by a single individual (as of when this was written), parts may be out of date and/or have typos and/or have other issues. Documentation is a challenge and I will do my best to maintain and update the documentation, while also working on new features.

____________

Finally, I am using a program called Abricotine for all my markdown editing. If you want to experience the documentation exactly how it was written, then I would highly recommend viewing the documentation with Abricotine. That said, any markdown editor/viewer should work just fine the with the documentation.
