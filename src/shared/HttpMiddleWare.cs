using System.Collections;
using System.Net;

namespace SMDB;

public delegate Task HttpMiddleware(HttpListenerRequest req, HttpListenerResponse res, Hashtable options);
public class HttpContext
{

}