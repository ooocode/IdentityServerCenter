/**
 * 上传文件到服务器
 * @param {any} url
 * @param {any} data
 * @param {any} cb   function (axiosres){}
 */
function uploadToServer(url, data, cb) {
    //上传图片到服务器
    var formData = new FormData();
    formData.append('file', data);
    axios.post(url, formData, {
        method: 'post',
        headers: {
            'Content-Type': 'multipart/form-data'
        },
        transformRequest: [function (data) {
            return data;
        }],
        onUploadProgress: function (e) {
            var v = ((e.loaded * 100) / e.total) || 0;
            var percentage = Math.round(v);
            if (percentage < 100) {
                swal('上传进度' + percentage + '%');
            }
        }
    }).then(function (resp) {
        cb(resp);
    }).catch(function (error) {
        console.log(error.response);
        swal("上传到服务器发生了错误,http状态码【" + error.response.status + '】', error.response.data, "error");
    });
}

/**
 * 压缩文件
 * @param {any} file  文件
 * @param {any} successCb   成功回调 function (file){}
 */
function CompressorFile(rawFile, successCb) {
    var options = {
        strict: true,
        checkOrientation: true,
        maxWidth: 800,
        maxHeight: undefined,
        minWidth: 0,
        minHeight: 0,
        width: undefined,
        height: undefined,
        quality: 0.6,
        mimeType: '',
        convertSize: 5000000,
        success: function (file) {
            var preSize = rawFile.size;
            var nowSize = file.size;

            //压缩后小于压缩前
            if (nowSize < preSize) {
                successCb(file);
            } else {
                successCb(rawFile);
            }
        },
        error: function (errmessage) {
            swal('错误', errmessage.message, "error");
        }
    };

    new Compressor(rawFile, options);
}