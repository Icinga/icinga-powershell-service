# Installation

The service itself does not ship with any installation tools or commands and entirely relies on the [Icinga PowerShell Framework](https://icinga.com/docs/windows) being installed. The Framework will provide a bunch of tools to make this step easy and user friendly.

## Using the Wizard

The most straight-forward way to install the service is by using the [Kickstart Script](https://icinga.com/docs/windows/latest/doc/installation/01-Kickstart-Script/) or the installation wizard of the Framework. In case you already run the wizard during installation, you most likely have chosen to install the service already. If not you can of course install the service manually.

## Manual installation

In order to manually install the service, you can use the [Icinga PowerShell Framework](https://icinga.com/docs/windows) and the provided Cmdlets provided. There are two commands available dealing with the download and installation.

### Download the Service binary

At first we can run `Get-IcingaFrameworkServiceBinary` to start the download wizard asking a bunch of questions. By default, the Wizard will download the `.zip` archive directly from Github, you can however also specify a custom location.

Please ensure for custom locations to enter the full path to the `.zip` file, like `https://github.com/Icinga/icinga-powershell-service/releases/download/v1.0.0/icinga-service-v1.0.0.zip`.

Start the service wizard asking us questions about location and install destination:

```powershell
$ServiceData = Get-IcingaFrameworkServiceBinary;
```

For a more automated approach you can skip the entire wizard by providing an answer to both the download url and installation question as argument:

```powershell
$ServiceData = Get-IcingaFrameworkServiceBinary -FrameworkServiceUrl 'https://github.com/Icinga/icinga-powershell-service/releases/download/v1.0.0/icinga-service-v1.0.0.zip' -ServiceDirectory 'C:\Program Files\icinga-framework-service\';
```

**Note:** For the next step it is important that you store the output of `Get-IcingaFrameworkServiceBinary` inside a variable for later usage, like `ServiceData` in our example.

### Install the service

Now as we downloaded the Service binary and placed it into the correct location in the step above, we can use `Install-IcingaFrameworkService` for the actual installation. The most easiest part is to store the result of `Get-IcingaFrameworkServiceBinary` into a variable, like we did in our example with `ServiceData`. We can now simply access the content of `ServiceData` and provide the arguments to our Cmdlet. In addition we will have to specify which user the service will be running with. This can either be a custom local/domain user or one of the default Windows service users

* NT AUTHORITY\NetworkService
* NT AUTHORITY\LocalService
* NT Authority\SYSTEM

```powershell
Install-IcingaFrameworkService -Path $ServiceData.ServiceBin -User 'NT AUTHORITY\NetworkService';
```

**Note:** For default Windows service users the `Password` argument is not required. This will only apply to local/domain users. Please note that `Password` is a secure string and you will have to parse it like this. The most easiest way is to use the dialog box for entering credentials and then using the returned object as arguments:

```powershell
$cred = Get-Credential;
Install-IcingaFrameworkService -Path $ServiceData.ServiceBin -User $cred.UserName -Password $cred.Password;
```

As different method you could also use the Icinga custom Cmdlet `ConvertTo-IcingaSecureString` for converting a `String` to a `SecureString` which might be handy for automated installations:

```powershell
$SecurePassword = ConvertTo-IcingaSecureString 'my_secret_password';
Install-IcingaFrameworkService -Path $ServiceData.ServiceBin -User 'my_user' -Password $SecurePassword;
```

**Important:** For security reasons we **do not** recommend to use direct shell inputs on the PowerShell for entering the password. Please make always sure to use a method to make it impossible to read a password from the command line history!

For example you could use `Read-Host` with the `AsSecureString` argument:

```powershell
$SecurePassword = Read-Host -Prompt 'Enter Service password' -AsSecureString;
Install-IcingaFrameworkService -Path $ServiceData.ServiceBin -User 'my_user' -Password $SecurePassword;
```

Regardless of the steps above, if everything works properly the service is now installed onto the system with the name `icingapowershell`.

### Checking Service state

To validate if the installation was successful you can check if the service is installed and running by using `Get-Service`:

```powershell
Get-Service 'icingapowershell';
```

```powershell
Status   Name               DisplayName
------   ----               -----------
Running  icingapowershell   Icinga PowerShell Service
```

### Installing background daemons

As we are now finished with the service installation we can make use of it by [registering background daemons](https://icinga.com/docs/windows/latest/doc/service/02-Register-Daemons/).
