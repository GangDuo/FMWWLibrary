
/**
 * 数値の3桁ごとに入るカンマを削除する
 */
if (!String.prototype.removeCommasInEvery3Digit) {
    String.prototype.removeCommasInEvery3Digit = function () {
        return this.replace(/,([0-9]{3})/g, "$1");
    };
}
/**
 * 店舗売上入力リスト
 * html to csv
 */
(function (undefined) {
    var isBusy = function () {
        return 'none' != document.getElementById('list:MSGDIV').style.display;
    };

    var parser = (function () {
        var _tableHeader = [];
        var _tableSubHeader;
        var _rows = [];
        var _header = [];

        var _onTableHeader = function (self) {
            if (0 < _tableHeader.length) {
                return;
            }
            var elms = self.getElementsByTagName('span');
            for (var i = 0; i < elms.length; ++i) {
                _tableHeader.push(elms[i].innerHTML);
            }
        };

        var _onTableSubHeader = function (self) {
            _tableSubHeader = {};
            var h = [];
            var elms = self.getElementsByTagName('span')[0].getElementsByTagName('span');
            for (var i = 0; i < elms.length; ++i) {
                var myArray = /([^：]*)：\[([^\]]*)\]/.exec(elms[i].innerHTML);
                var itemArray = myArray[2].split(' ');
                if (1 === itemArray.length) {
                    _tableSubHeader[myArray[1]] = myArray[2];
                } else {
                    _tableSubHeader[myArray[1] + 'CD'] = itemArray[0];
                    _tableSubHeader[myArray[1]] = itemArray[1];
                }
                if (_header.length === 0) {
                    if (1 === itemArray.length) {
                        h.push(myArray[1]);
                    } else {
                        h.push(myArray[1] + 'CD');
                        h.push(myArray[1]);
                    }
                }
            }
            if (_header.length === 0) {
                for (var i = 0; i < h.length; ++i) {
                    _header.push(h[i]);
                }
                for (var i = 0; i < _tableHeader.length; ++i) {
                    _header.push(_tableHeader[i]);
                }
            }
        };

        var _onItem = function (self) {
            var itemArray = [];
            for (var i in _tableSubHeader) {
                itemArray.push(_tableSubHeader[i]);
            }
            var elms = self.getElementsByTagName('span');
            for (var i = 0; i < elms.length; ++i) {
                itemArray.push(elms[i].innerHTML.removeCommasInEvery3Digit());
            }
            _rows.push(itemArray);
        };

        var _onSubtotal = function (self) {
            var elms = self.getElementsByTagName('span');
            for (var i = 0; i < elms.length; ++i) {
                //console.log(elms[i].innerHTML);
            }
        };

        var _parse = function () {
            var tables = document.getElementById('list:maintables').getElementsByTagName('table');
            for (var j = 0; j < tables.length; ++j) {
                var trs = tables[j].getElementsByTagName('tr');
                for (var i = 0; i < trs.length; ++i) {
                    switch (trs[i].className) {
                        case 'tableHeader':
                            _onTableHeader(trs[i]);
                            break;
                        case 'tableSubHeader':
                            _onTableSubHeader(trs[i]);
                            break;
                        case 'depth0even':
                        case 'depth0odd':
                            _onItem(trs[i]);
                            break;
                        case 'depth1':
                            _onSubtotal(trs[i]);
                            break;
                        default:
                            break;
                    }
                }
            }
        };
        return {
            header : _header,
            rows   : _rows,
            parse  : _parse
        };
    })();

    var DELAY_MILLISECOND = 1000;
    setTimeout(function () {
        if (isBusy()) {
            setTimeout(arguments.callee, DELAY_MILLISECOND);
            return;
        }
        parser.parse();
        var csv = parser.header + "\n";
        var rows = parser.rows;
        for (var i = 0; i < rows.length; ++i) {
            csv += rows[i] + "\n";
        }
        window.external.Completed(csv);
    }, DELAY_MILLISECOND);
})();
