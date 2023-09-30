Summary of the scenario that is expected to be followed when testing the project on the Swagger side:

1- 2 parameters can be sent to the UrlShorten method. The first of these is "longUrl", which is the long URL that is expected to be shortened, and it is mandatory to write it.

The second parameter must be written if you want to create CustomUrl.

If the first parameter (longUrl) is written and the other parameter, customUrl, is left blank, a normal short url is created and a status code of 200 is returned.

If the second parameter is also written; In this case, the short url specified with "customUrl" is created and saved in the database.



2- shortUrl is sent as a parameter to the Url /ShortUrl method. The same shorturl must be written in both fields. it will show the long address it wants to go within status 200.

The Cors part has been left as is for easier testing.

Hamit Yıldırım
18.09.2023
