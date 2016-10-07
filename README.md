# Mailzory
Mailzory helps you to send emails which are based on Razor templates. Mailzory is dependent on RazorEngine project for compiling your Razor templates and that means you are limited to the power of RazorEngine for template processing.
* Mailzory sends HTML (Razor Based) Templates
* Mailzory supports CC & BCC 
* Mailzory supports attachments
* Mailzory supports custom sender's display name
* Mailzory supports synchronous and asynchronous (Task Based) sending
* For the purpose of configuring Smtp settings, You can inject custom SmtpClient instance at Mailzory constructor. Also note that the other way for configuring Smtp settings would be using [mailSettings](https://msdn.microsoft.com/en-us/library/w355a94k(v=vs.110).aspx) tag in web.config or app.config.

## Install (Nuget)

```powershell
Install-Package Mailzory

```
## Usage

Suppose we have the following loosely typed template. ("hello.cshtml")

```html
<div>

    <h1 style="font-size:13px">
        <label>@ViewBag.Name</label>
    </h1>

    <hr>

    <div>
        <p>
            @ViewBag.Content
        </p>       
    </div>

</div>
```

And also suppose that we have the following strongly typed template.( "typedHello.cshtml" )

```html

@model Test.MessageModel
<div>

    <h1 style="font-size:13px">
        <label>@Model.Name</label>
    </h1>

    <hr>

    <div>
        <p>
            @Model.Content
        </p>
    </div>

</div>

```

### Sendnig synchronous email (Loosely Typed Template)

Mailzory uses SmtpClient for sending emails, but as you can see there is no sign of SmptClient in the following snippet. At the following sample Mailzory uses [mailSettings](https://msdn.microsoft.com/en-us/library/w355a94k(v=vs.110).aspx) in web.config (or app.config) for configuring the SmtpClient.

```c#
// template path
var viewPath = Path.Combine("Views/Emails", "hello.cshtml");
// read the content of template and pass it to the Email constructor
var template = File.ReadAllText(viewPath);

var email = new Email(template);

// set ViewBag properties (you can also use strongly typed models)
email.ViewBag.Name = "Johnny";
email.ViewBag.Content = "Mailzory Is Funny";

// set Attachments (Optional)
email.Attachments.Add(new Attachment("Attachments/attach1.pdf"));
email.Attachments.Add(new Attachment("Attachments/attach2.docx"));

// set your desired display name (Optional)
email.SetFrom("mailzory@outlook.com","Wolf of Wall Street");

// send email
email.Send("mailzory@outlook.com", "subject");

// send email with CC
email.Send("mailzory@outlook.com", "subject",
    ccMailAddresses:new[] { "ehsan.mir2000@gmail.com" });

// send email with BCC
email.Send("mailzory@outlook.com", "subject",
    bccMailAddresses: new[] { "mailzory@gmail.com" });
```

A sample for mailSettings at web.config (or app.config)

```xml

  <system.net>
    <mailSettings>
      <smtp deliveryMethod="Network" from="{the mail address which is sending your emails: mailzory@outlook.com}">
        <network enableSsl="{true|false}" host="{mail server address}" port="{mail server port}" defaultCredentials="{true|false}" userName="{username}" password="{password}" />
      </smtp>
    </mailSettings>
  </system.net>
  
```

### Sendnig asynchronous email (Loosely Typed Template)

You can send emails asynchronously. Mailzory async methods are return a Task instance.

```c#
// template path
var viewPath = Path.Combine("Views/Emails", "hello.cshtml");
// read the content of template and pass it to the Email constructor
var template = File.ReadAllText(viewPath);

var email = new Email(template);

// set ViewBag properties
email.ViewBag.Name = "Johnny";
email.ViewBag.Content = "Mailzory Is Funny";

// send email
var task = email.SendAsync("mailzory@outlook.com", "subject");
task.Wait();
```

### Sending asynchronous email (Strongly Typed Templates)

In addition to ViewBag, you can pass a strongly typed model to your template. Note that in the following example we are sending an email to multiple recievers.

```c#
var viewPath = Path.Combine("Views/Emails", "typedHello.cshtml");
// read the content of template and pass it to the Email constructor
var template = File.ReadAllText(viewPath);
// fill the model
var model = new MessageModel
{
    Content = "Mailzory Is Funny. Its a Model Based message.",
    Name = "Johnny"
};

var email = new Email<MessageModel>(template,model);

// send email
var task =
    email.SendAsync(new[] { "mailzory1@mailzory.co","mailzory2@mailzory.co" }
    , "subject");

task.Wait();
```
### Configure SmtpClient At Runtime

You can pass a customized instance of SmtpClient to the Email constructor and Mailzory will use that for sending emails instead of dependeing on mailSettings at web.config.

```c#
var smtpClient = new SmtpClient("host")
{
    EnableSsl=true,
    Host="",
    Timeout=60,
    UseDefaultCredentials=false,
    Port=587,
    Credentials = new NetworkCredential("username", "password")
};
// template path
var viewPath = Path.Combine("Views/Emails", "hello.cshtml");
// read the content of template and pass it to the Email constructor
var template = File.ReadAllText(viewPath);
var email = new Email(template, smtpClient);
email.Send("test@outlook.com", "subject");

```
