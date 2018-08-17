define(function () {
    return {
        loadLink: function (url, rel, type) {
            var link = document.createElement("link");
            if (!!type) {
                link.type = type;
            }
            link.rel = rel;
            link.href = url;
            document.getElementsByTagName("head")[0].appendChild(link);
        },
        loadScript: function (url, callback) {
            var script = document.createElement("script")
            script.type = "text/javascript";
            if (script.readyState) {
                script.onreadystatechange = function () {
                    if (script.readyState == "loaded" || script.readyState == "complete") {
                        script.onreadystatechange = null;
                        if (typeof (callback) === 'function') {
                            callback();
                        }
                    }
                };
            } else {
                script.onload = function () {
                    if (typeof (callback) === 'function') {
                        callback();
                    }
                };
            }
            script.src = url;
            document.getElementsByTagName("head")[0].appendChild(script);
        },
        isUrl: function (url) {
            var r = new RegExp('^(?:[a-z]+:)?//', 'i');
            return r.test(url);
        }    
    };
});