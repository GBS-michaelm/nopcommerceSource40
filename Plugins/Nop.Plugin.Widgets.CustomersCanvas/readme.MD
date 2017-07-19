# How to build a plugin

Follow these steps:

1. Download the nopCommerce source code from their [Download page](http://www.nopcommerce.com/downloads.aspx). Be sure that you download the version 3.8.
2. Unzip it.
3. Put the source code of our plugin to the `Plugins` folder.
4. Open the entire nopCommerce solution in Visual Studio.
5. Build the solution.
6. Add our plugin project to the solution (to the Plugins folder). Make sure that you select a correct target .NET Framework version (4.5.1). 
7. Build the project. It should download the Nuget dependencies automatically. Also, it depends on other projects in the nopCommerce solution - if it cannot find dependencies, try to rebuild other projects from the Presentation and Library folders. 
8. It is configured to put the compiled plugin binaries to `Presentation\Nop.Web\Plugins` folder (so that you could run the project in Visual Studio and it is automatically appear there), however, if you prefer to do it to another folder, you can change it in the `Project Settings` -> `Build` -> `Output path`.

The project is created with Visual Studio 2015. It is possible to build it with Visual Studio 2013, but you should install the latest C# compiler. To do it, execute the following line in the Package Manager Console:

```
Install-Package Microsoft.Net.Compilers
```

# How to install a plugin

After you build the project, you need to follow these steps to get it working:

1. If you did not do it yet, deploy the nopCommerce to your server.
2. Copy the folder, created by Visual Studio to `Plugins` folder of your nopCommerce web installation. 
3. Sign in as administrator and go to the Administration panel.
4. Go to *Configuration* -> *Plugins* -> *Local Plugins* section.
5. Click the *Reload list of plugins* button.
6. Find *CustomersCanvas* by Aurigma in the list and click *Install*.
7. Click *Edit* and select *Is enabled* checkbox and click *Save*. 
8. Click *Configure* and put a URL of your Customer's Canvas installation.

When you are done, you can go to *Configuration* -> *CustomersCanvas Editors* page and configure the plugin here. 

# Editors

To get the plugin working, you should have at least one Editor - a JavaScript application, created in accordance to a particular rules. Once you get it, you can put it to the *Editors* folder (in its own subfolder). You can also manage editors through the web interface, but it is an experimental functionality yet. 

Once you deployed an Editor with its configurations, create a new Product (or open the existing one in the Catalog management section). You will notice the *Customer's Canvas Settings* tab. Here you can select the editor and a configuration and click Add Attribute button. Now the product is associated with the editor. 

Note, if the editor requires certain Product or Specification Attributes, don't forget to add them.  

If everything is done correctly, on the product page you will see the Design Product button. When you click it, the modal window opens up and the editor's user interface is rendered there. 
