# Systemutvecklingsprojekt - Grupp Sinder
Forked from -> [niklastheman:Systemutvecklingsprojekt](https://github.com/niklastheman/Systemutvecklingsprojekt/).
- Members
	- Jakob
	- Des
	- Golden
	- Silver
	- Tumba

## Installation
1. Add an App.config file with containing the following;
XXX = secrets
```
  <connectionStrings>
    <add name="tjackobacco.com" connectionString="server=XXX;port=XXX;database=XXX;uid=XXX;pwd=XXX;"/>
  </connectionStrings>
  <appSettings>
    <add key="secret" value="XXX" />
  </appSettings>
```

## Run
```
// For live reload use
dotnet watch run

// Else just run normally from Visual Studio or whatever 
```

## Succes box
```
Successbox.element.classList.add('bg-success')
Successbox.setColor('white')
Successbox.show("nya lösenord stämmer inte överens, "Titeln")
```

## Danger box
```
Successbox.element.classList.add('bg-danger')
Successbox.setColor('white')
Successbox.show("nya lösenord stämmer inte överens, "Titeln")
```
