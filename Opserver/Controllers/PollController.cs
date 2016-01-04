﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using StackExchange.Opserver.Data;
using StackExchange.Opserver.Helpers;
using StackExchange.Opserver.Models;

namespace StackExchange.Opserver.Controllers
{
    [OnlyAllow(Roles.Authenticated)]
    public class PollController : StatusController
    {
        [Route("poll")]
        public ActionResult PollNodes(string type, string[] key, Guid? guid = null)
        {
            if (type.IsNullOrEmpty())
                return JsonError("type is missing");
            if (!(key?.Any() ?? false))
                return JsonError("key is missing");
            try
            {
                if (key.Length > 1)
                {
                    bool result = true;
                    Parallel.ForEach(key, k =>
                    {
                        result &= PollingEngine.Poll(type, k, guid, sync: true);
                    });
                    return Json(result);
                }
                var pollResult = PollingEngine.Poll(type, key[0], guid, sync: true);
                return Json(pollResult);
            }
            catch (Exception e)
            {
                return JsonError("Error polling node: " + e.Message);
            }
        }

        [Route("poll/all"), HttpPost, OnlyAllow(Roles.GlobalAdmin)]
        public ActionResult PollDown()
        {
            try
            {
                PollingEngine.PollAll(true, true);
                return Json(true);
            }
            catch (Exception e)
            {
                return JsonError("Error polling all nodes: " + e.Message);
            }
        }
    }
}