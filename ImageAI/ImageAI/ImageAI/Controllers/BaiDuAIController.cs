using System;
using System.Collections.Generic;
using System.IO;
using Baidu.Aip.ImageClassify;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace ImageAI.Controllers
{
    [Route("BaiDu")]
    [ApiController]
    public class BaiDuAIController : Controller
    {
        private readonly string APP_ID = "16546407";
        private readonly string API_KEY = "nhqDlGK6lMqxVOYNWdGBZSo7";
        private readonly string SECRET_KEY = "h9Ih47YAI6pOSBXNDS145EqEmz2SqLwj";

        private readonly ImageClassify imageClassify = null;
        public BaiDuAIController()
        {
            imageClassify = new ImageClassify(API_KEY, SECRET_KEY)
            {
                Timeout = 10000
            };
        }
        [HttpPost]
        [Route("ImagesUpload")]
        public JsonResult ImagesUpload()
        {
            var files = HttpContext.Request.Form.Files["sunqiangFile"];
            var fileName = files.FileName;
            string currentPictureExtension = Path.GetExtension(files.FileName).ToUpper(); //获取图片后缀
            var new_path = DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid() + "-" + files.FileName.ToUpper();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", new_path);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                //再把文件保存的文件夹中
                files.CopyTo(stream);

            }
            var imagesbyte = System.IO.File.ReadAllBytes(path);
            //var result = imageClassify.AdvancedGeneral(imagesbyte);
            // 如果有可选参数
            var options = new Dictionary<string, object> { { "baike_num", 5 } };
            // 带参数调用通用物体识别
            var result = JsonConvert.DeserializeObject<Root>(imageClassify.AdvancedGeneral(imagesbyte, options).ToString());
            var data = result.result.OrderByDescending(x => x.score).First();
            return new JsonResult(data);
        }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
    }
}

public class Baike_info
{
    /// <summary>
    /// 
    /// </summary>
    public string baike_url { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string image_url { get; set; }
    /// <summary>
    /// 垃圾箱是存放垃圾的容器，作用与垃圾桶相同，一般是方形或长方形。现在流行一种广告型垃圾箱.普遍用于小区，公园等公共场所。垃圾箱会发展成一种新型的智能吸气型垃圾桶。其奥妙就在于，在社区每个垃圾桶的下面都有一个地下暗道和气压机，如同下水道一样，在气压机的作用下。
    /// </summary>
    public string description { get; set; }
}

public class ResultItem
{
    /// <summary>
    /// 
    /// </summary>
    public double score { get; set; }
    /// <summary>
    /// 商品-公共设施
    /// </summary>
    public string root { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Baike_info baike_info { get; set; }
    /// <summary>
    /// 垃圾箱
    /// </summary>
    public string keyword { get; set; }
}

public class Root
{
    /// <summary>
    /// 
    /// </summary>
    public string log_id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int result_num { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<ResultItem> result { get; set; }
}