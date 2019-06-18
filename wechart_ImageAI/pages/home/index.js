const app = getApp();
Page({
  data: {
    StatusBar: app.globalData.StatusBar,
    CustomBar: app.globalData.CustomBar,
    tempFilePaths: '',
    text:'',
  },
  onLoad: function () {
  },
  chooseimage: function () {
    var that = this;
    that.setData({
      text:'',
      categoryName:''
    }) 

    wx.showActionSheet({
      itemList: ['从相册中选择', '拍照'],
      itemColor: "#CED63A",
      success: function (res) { 
        if (!res.cancel) {
          if (res.tapIndex == 0) {
            that.chooseWxImage('album')
          } else if (res.tapIndex == 1) {
            that.chooseWxImage('camera')
          }
        }
      }
    })
  },
  chooseWxImage: function (type) {
    var that = this;
    wx.chooseImage({
      sizeType: ['original', 'compressed'],
      sourceType: [type],
      success: function (res) { 
        wx.showLoading({
          title: '大队长正在努力识别中',
        })
        wx.uploadFile({
          url: 'http://localhost:51988/BaiDu/ImagesUpload', //仅为示例，非真实的接口地址
          filePath: res.tempFilePaths[0],
          name: 'sunqiangFile',
          formData: {
            'user': 'test'
          },
          success: function (res) {           
            wx.hideLoading();
            var data = res.data
            var datas = JSON.parse(data)
            var categoryName='干垃圾'
            if (datas.baike_info != '' && datas.baike_info.description!=''){
              that.setData({
                description: datas.baike_info.description
              });
            }
            that.setData({
              text: datas.root + '-' + datas.keyword,
              categoryName: categoryName
            }) 
          
          }
        })         
        that.setData({
          tempFilePaths: res.tempFilePaths[0]          
        })
      }
    })
  }
}) 