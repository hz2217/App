# week7 - 网络访问

[Reference-Thanks very much](https://blog.csdn.net/yaoxh6/article/details/80056321)

1. 找到合适的 API，并申请 key

2. 用 [website_json](http://json2csharp.com/#) [website_xml](http://xmltocsharp.azurewebsites.net/)将示例的 json or xml 格式，转换成类

3. 在项目中添加相应类，注意 json 类 需要添加 [Datamember] 等修饰

4. 设计 UI

5. 写网络访问处理函数

   ```C#
   xml
   public static async Task<CoordAddressResult> GetWeather(String Location, String Location2)
   {
   	string link = "http://api.avatardata.cn/CoordAddress/Lookup?key=9268d181665640499b8fa1a0f44ad0da&lat=" + Location + "&dtype=XML&lon=" + Location2;
   	var http = new HttpClient();
       var response = await http.GetAsync(link);
       var result = await response.Content.ReadAsStringAsync();
       var serializer = new XmlSerializer(typeof(CoordAddressResult));

   	var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
       var data = (CoordAddressResult)serializer.Deserialize(ms);
       return data;
   }
   ```

   ```C#
   json
   public async static Task<RootObject> GetGeoInformation(String str)
   {
   	//Create an HTTP client object
   	Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
   	//Add a user-agent header to the GET request. 
   	var headers = httpClient.DefaultRequestHeaders;
   	//The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
   	//especially if the header value is coming from user input.
   	string header = "ie";
       if (!headers.UserAgent.TryParseAdd(header))
       {
       	throw new Exception("Invalid header value: " + header);
   	}

       header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
       if (!headers.UserAgent.TryParseAdd(header))
       {
       	throw new Exception("Invalid header value: " + header);
       }

       Uri requestUri = new Uri("http://api.avatardata.cn/Weather/Query?key=fa848569705941a1abdf5c83a6e30f6c&cityname=" + str);
       //Send the GET request asynchronously and retrieve the response as a string.
       Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
   	string httpResponseBody = "";
   	try
       {
       	//Send the GET request
           httpResponse = await httpClient.GetAsync(requestUri);
           httpResponse.EnsureSuccessStatusCode();
           httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
       }
   	catch (Exception ex)
       {
       	httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
       }
   	var serializer = new DataContractJsonSerializer(typeof(RootObject));
   	var ms = new MemoryStream(Encoding.UTF8.GetBytes(httpResponseBody));
   	RootObject data = (RootObject)serializer.ReadObject(ms);
   	return data;
   }
   ```

6. Button 点击事件

   ```C#
   json
   private async void SearchWeatherButton_ClickAsync(object sender, RoutedEventArgs e)
   {
   	string input = SearchInputWeather.Text;
       // 注意这个 特殊的 RootObject 类，是整个文件的类包含的起始
   	RootObject data = await OpenGeoProxy.GetGeoInformation(input);
   	string result = "| : Cityname: " + input + "\n| : "
   			+ "Humidity: " + data.result.realtime.weather.humidity + "\n| : "
   			+ "Img: " + data.result.realtime.weather.img + "\n| : "
   			+ "Info: " + data.result.realtime.weather.info + "\n| : "
   			+ "Temperature: " + data.result.realtime.weather.temperature;
       // 注意学习上面的访问方式 data.result.realtime.weather.temperature
   	Weather.Text = result;
   }
   ```

   ```C#
   xml
   private async void SearchAddressButton_ClickAsync(object sender, RoutedEventArgs e)
   {
   	string input = SearchInputAddress.Text;
   	string input2 = SearchInputAddress2.Text;
   	CoordAddressResult data = await xmlway.GetWeather(input, input2);
   	string result = "| : Cityname: " + input + "\n| : "
   			+ "Country: " + data.Result.Country + "\n| : "
   			+ "Country_code: " + data.Result.Country_code + "\n| : "
   			+ "Province: " + data.Result.Province + "\n| : "
   			+ "City: " + data.Result.City + "\n| : "
   			+ "District: " + data.Result.District + "\n| : "
   			+ "Street: " + data.Result.Street + "\n| : "
   			+ "Street_number: " + data.Result.Street_number;
   	Address.Text = result;
   }
   ```

   ​