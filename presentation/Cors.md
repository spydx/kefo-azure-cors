---
marp: true
theme: bouvet
paginate: true
---

<!-- _class: lead -->

# CORS

## Cross-Origin Resource Sharing

Kenneth Fossen

---

# What is this?

```javascript
<img src="http://bank.com/transfer.do?acct=MARIA&amount=100000" width="0" height="0" border="0">
```

---

# Is this also XSS?

```javascript
<script>
function put() {
    var x = new XMLHttpRequest();
    x.open("PUT","http://bank.com/transfer.do",true);
    x.setRequestHeader("Content-Type", "application/json");
    x.send(JSON.stringify({"acct":"BOB", "amount":100})); 
}
</script>

<body onload="put()">
```
<!-- _footer: 'https://owasp.org/www-community/attacks/csrf' --->
---

# Common factors for these attacks

![bg right:40% 80%](assets/Don_Knotts_Barney_and_the_bullet_Andy_Griffith_Show.jpg)

- They both use Javascript in the browser
- These attacks are used to
  - Collect user cookies/secrets
  - Act on the user's behalf (without them knowing)
- CSRF is also known as the Confused Deputy

<!-- _footer: 'Confused deputy problem. (2022, August 5). In Wikipedia. https://en.wikipedia.org/wiki/Confused_deputy_problem' -->

---

# Welcome CORS

- The modern browser now:
  - Restricts access to Cookies for JS
  - Content Security Policy to enable/disable APIs Javascript has access to
  - Uses CORS to restrict where we can download resources

- CORS helps unless you do stuff like this
  - `Access-Control-Allow-Origin: *`

---

# TODAY'S PROBLEM?

Where is CORS in this?

![bg right:60% 90%](assets/commonlib.jpg)

---

# Agenda

- Repetition: Origin
- CORS Deep Dive
- Azure App Service CORS
- ASP.NET WebAPI CORS
- Backend-For-Frontend (BFF)

---

# Repetition: Origin

What is Origin in HTTP world?

It is a tuple consisting of `<protocol>://<host>:<port>`

```sh
https://www.kefo.no/
http://www.kefo.no:443/
https://kefo.no/boisys/blog/today_i_hiked_photos
http://blog.kefo.no/
http://blog.kefo.no:81
```

---

# CORS Deep Dive

_"An HTTP-Header mechanism that allows a **server** to indicate any origins other than its **own** from which a browser should be permitted to be loading resources."_

---
# OVERVIEW
## CORS

![bg right:65% 80%](assets/cors_principle.jpg)

---

# A Simple Request IS

- A `GET`, `HEAD`, or `POST` request

That only allows the following headers:

- `Accept`, `Accept-Language`, `Content-Language`, `Range`, and `Content-Type`

`Content-Type` can only hold these values:

- `application/x-www-form-urlencoded`
- `multipart/form-data`
- `text/plain`

<!-- _footer: 'https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS' -->

---

# That was easy

## Yeah BUT, Safari, Firefox, and Chrome do implement things differently! 🤯🤦🏻‍♂️

- Firefox does not implement the `Range`.
- Safari/WebKit is using stricter versions of `Accept`, `Accept-Language`, and `Content-Language`.

---

# CORS: Preflight Request

- The browser sends an OPTIONS request
- If Response is:
  - HTTP 204 or 200 - Everything OK
  - HTTP 405 - Not allowed BAD

![bg right:45% 80%](assets/preflight_correct.png)

---

```
❯ curl -X OPTIONS --location "http://localhost:5220/Bar" \
    -H "Origin: "https://local.kefo.no"" \
    -H "Access-Control-Request-Method: POST" -v
*   Trying 127.0.0.1:5220...
* Connected to localhost (127.0.0.1) port 5220 (#0)
> OPTIONS /Bar HTTP/1.1
> Host: localhost:5220
> User-Agent: curl/8.0.1
> Accept: */*
> Origin: https://local.kefo.no
> Access-Control-Request-Method: POST
> 
< HTTP/1.1 204 No Content
< Date: Tue, 21 Mar 2023 21:23:03 GMT
< Server: Kestrel
< Access-Control-Allow-Methods: POST
< Access-Control-Allow-Origin: https://local.kefo.no
```

---

# Credential Request

are requests that contain:

- HTTP Cookie
- HTTP Authentication information.

![bg right:50% 80%](assets/cred-req-updated.png)

---

# Credential Request Cont.

- Not all are pre-flighted (e.g. a `GET` some requests)
- The server must specify: `Access-Control-Allow-Origin` without `*`
- The client (browser) will reject the *RESPONSE*
  if `Access-Control-Allow-Credentials: true` is missing.

---

# Azure App Service CORS

- Enabled or disabled?
- What does this checkbox mean?
  ![bg right:60% 90%](assets/kefo-azure-cors.png)

---


# Azure Default Policy

- Takes Precedence

For the list of Allowed Origins this is the policy:

- `AllowAllHeaders`
- `AllowAnyMethods`
- `IncludeCredentials` (selectable)

---

# Azure Defaults

Returns

- 200 OK for ok requests (204)
- 400 Bad Request for non-compliant requests (405)

How to remove

- WebPage: unclick and remove all origins
- Cli: `az webapp cors remove --allow-origins -g off-cors -n kefo-azure-cors-settings `


---

# ASP.NET WebAPI CORS

Is defined through:

- Middleware `Program.cs`
- Controller `Attributes`

---
# Configurations

Only use:

- Attributes `[EnableCors("policyname")]`
- Middleware `app.UseCors()`

![bg right:50% 80%](assets/middleware-pipeline.svg)

---
# Example: Middleware Order

```
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
...
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// app.UseCookiePolicy();

app.UseRouting();
// app.UseRequestLocalization();
// app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
// app.UseSession();
// app.UseResponseCompression();
// app.UseResponseCaching();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```
---

# Example Policy

```c#
// configure Cors Policy
builder.Services.AddCors(
    p => p.AddDefaultPolicy(
        settings => settings
            .WithOrigins("https://local.kefo.no")
            .AllowAnyHeader()
            .AllowAnyMethod())
    );
app.UseCors();
```
```c#
// configure Cors Policy
var _policyName = "localkefo"; 
builder.Services.AddCors(
    p => p.AddPolicy(name: _localkefo,
        settings => settings
            .WithOrigins("https://local.kefo.no")
            .AllowAnyHeader()
            .AllowAnyMethod())
    );
app.UseCors(_policyName);
```
---

# BFF

![bg right:60% 90%](assets/bff.jpg)
<!-- _footer: 'https://www.oreilly.com/library/view/building-microservices-2nd/9781492034018/' --->
---

# Sources

- [Mozilla CORS](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
- [Azure Specifics](https://learn.microsoft.com/en-gb/azure/app-service/app-service-web-tutorial-rest-api)
- [Implement CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0)
- [Tutorial: Host RESTful API with CORS - Azure App Service](https://learn.microsoft.com/en-gb/azure/app-service/app-service-web-tutorial-rest-api)
- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0#middleware-order)
- [Fetch Standard](https://fetch.spec.whatwg.org/#cors-safelisted-request-header)
- [GitHub spydx/kefo-azure-cors](https://www.github.com/spydx/kefo-azure-cors)
