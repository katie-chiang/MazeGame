QSearch README
====================

Running QSearch
-----------------------------

Public Elastic IP is ``35.175.27.35`` <br/>
Used OAuth2.0 to enable the login feature for Google account <br/>
No Google APIs are used currently <br/>

### Remotely

In any browser, enter http://35.175.27.35:8080/ to access the website <br/>
The login link does not work when it is remotely hosted, but it works when ran locally <br/>

### Locally

Navigate to directory <download_path>/searchEngine/csc326/ <br/>
Run the Python file QSearch.py by typing in the terminal <br/>

```
$ python QSearch.py
```
In any browser, enter http://localhost:8080/ <br/>

Benchmark Setup
-----------------------------

Benchmarking was performed in order to analyze the performance of the web application. <br/>

### Benchmarking Tool

Installed Apache benchmarking tool, ab, on EC2 Ubuntu machine serving our application. <br/>
Using command: <br/>

```
$ sudo apt-get install apache2-utils
```

Run server using the above instructions for running application locally.
In another ssh session, while the server is running, enter command
To test max number of requests (results can be found in RESULT1.txt):

```
$ ab -n 40000 -c 1 -v 1 http://0.0.0.0:8080/?keywords=hello+world
```

To test maxmimum concurrency (results can be found in RESULT2.txt):
```
$ ab -n 400 -c 400 -v 1 http://0.0.0.0:8080/?keywords=hello+world
```
