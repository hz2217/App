# Week7 - 网络访问

[TOC]

**Reference**

[HttpWebRequest](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest(v=vs.80).aspx) [HttpClient](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx)

1. 添加命名空间

   ```C#
   using System.Net
   using System.Net.Http
   ```

2. 作业要求

   1. 使用HttpWebRequest或HttpClient访问网络
   2. 输入城市查天气，快递查询等生活实用功能至少完成一种
   3. 自行上网查找API（不可与Demo相同）
   4. Bonus：完成两种不同的API，且分别为JSON和XML格式

3. 学习 **HttpWebRequest**

   1. ```C#
      [SerializableAttribute] 
      public class HttpWebRequest : WebRequest, ISerializabl
      ```

   2. **HttpWebRequest** 类对 **WebRequest** 中定义的属性和方法提供支持，也对使用户能够直接与使用 HTTP 的服务器交互的附加属性和方法提供支持

   3. 不要使用 [HttpWebRequest](https://msdn.microsoft.com/zh-cn/library/w8s3z8zy(v=vs.80).aspx) 构造函数。使用 [System.Net.WebRequest.Create](https://msdn.microsoft.com/zh-cn/library/system.net.webrequest.create(v=vs.80).aspx) 方法初始化新的 **HttpWebRequest** 对象。如果统一资源标识符 (URI) 的方案是 `http://` 或 `https://`，则 **Create** 返回 **HttpWebRequest** 对象

   4. [GetResponse](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest.getresponse(v=vs.80).aspx) 方法 -> 向 [RequestUri](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest.requesturi(v=vs.80).aspx) 属性中指定的资源 -> 发出同步请求 -> 并返回包含该响应的 [HttpWebResponse](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebresponse(v=vs.80).aspx)

      可以使用 [BeginGetResponse](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest.begingetresponse(v=vs.80).aspx) 和 [EndGetResponse](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest.endgetresponse(v=vs.80).aspx) 方法对资源发出异步请求

   5. **HttpWebRequest** 将发送到 Internet 资源的公共 HTTP 标头值公开为属性，由方法或系统设置；下表包含完整列表。可以将 [Headers](https://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest.headers(v=vs.80).aspx) 属性中的其他标头设置为名称/值对。注意，服务器和缓存在请求期间可能会更改或添加标头

   6. URI http://www.contoso.com/. 创建 HttpWebRequest

      ```
      HttpWebRequest myReq =
      (HttpWebRequest)WebRequest.Create("http://www.ip138.com/sj/");
      ```

4. 学习 **HttpClient**

   1. `Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI`

5. **Install the Web API Client Libraries**

   1. From the **Tools** menu, select **NuGet Package Manager** > **Package Manager Console**. In the Package Manager Console (PMC), type the following command:

      `Install-Package Microsoft.AspNet.WebApi.Client`

   2. The preceding command adds the following NuGet packages to the project:

      1. Microsoft.AspNet.WebApi.Client
      2. Newtonsoft.Json

   3. Json.NET is a popular high-performance JSON framework for .NET.

6. **Add a Model Class**

   1. This class matches the data model used by the web API. An app can use **HttpClient** to read a `Product`instance from an HTTP response. The app doesn't have to write any deserialization code

      ```C#
      public class Product
      {
          public string Id { get; set; }
          public string Name { get; set; }
          public decimal Price { get; set; }
          public string Category { get; set; }
      }
      ```

      ​

