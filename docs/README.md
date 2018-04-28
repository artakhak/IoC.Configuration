# IoC.Configuration Documentation

This is a Sphinx project for IoC.Configuration documentation.
* The NuGet package IoC.Configuration can be downloaded at [https://www.nuget.org/packages/IoC.Configuration/](https://www.nuget.org/packages/IoC.Configuration/).
* The source code is at [https://github.com/artakhak/IoC.Configuration](https://github.com/artakhak/IoC.Configuration)

## Sphinx/ReadTheDocs documentation setup instructions

### Install python from https://www.python.org/download/releases/2.5/msi/.
    -After the installation completes, click on "Disable path length limit" button on last page.
	-Add the python installation folder (the one that contains python.exe), and the Scripts folder under installation folder to Path environment variable (if it is not added by installer)
    
    Execute the following commands in windows command
    python -m pip install -U pip #this step most probably will not be needed since pip should be in python installation)
    python -m pip install --upgrade pip
  
### Install Sphinx  (see http://www.sphinx-doc.org/en/master/).
    -Open regular command window and run the following commands
     import pip #this might not be required)
     python -m pip install -U sphinx
     python -m pip install sphinx sphinx-autobuild

### Create the documentation project with sphinx
    -In command window run the following commands:
    
    cd "k:\..\IoC.Configuration.Documentation" 
    sphinx-quickstart #this will build the Sphinx doc project. 
        Here are non-default responses:
            Separate source and build directories: Y.
            Project name: IoC.Configuration
            Do you want to use the epub builder: N
### Building the docs

      -In k:\..\IoC.Configuration.Documentation\source\conf.py set the value of html_theme to 'bizstyle'
      -In command window change to "k:\..\IoC.Configuration.Documentation directory" (see the section above), and
        run the following command:
        make html
       
       -To reload the docs, run 
        sphinx-autobuild . _build/html
        
### Publishing to Git
      -Before committing to git run "make clean" to clean the generated files. ReadTheDcos will build the docs.
        