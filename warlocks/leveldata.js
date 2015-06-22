"use strict";

function color(r, g, b, a) {
  this.r = r;
  this.g = g;
  this.b = b;
  this.a = a;
}

color.prototype.toInt = function () {

  return (this.a << 24) | (this.b << 16) | (this.g << 8) | this.r;

};

var LevelData = function (width, height, data, palette) {
  /*
  var _back = document.createElement('canvas');
  _back.width = "800px";//width + "px";
  _back.height = "600px";//height + "px";
  var _ctx = _back.getContext("2d");
  var _imageData = _ctx.getImageData(0, 0, 800, 600);
  */
  var _buffer = new ArrayBuffer(width * height * 4);
  var _wb = new Uint32Array(_buffer);
  var _rb = new Uint8ClampedArray(_buffer);

  var _screenbuffer = new ArrayBuffer(800 * 600 * 4);
  var _screenview = new Uint8ClampedArray(_screenbuffer);
  var _screenwrite = new Uint32Array(_screenbuffer);
  var _imageData = new ImageData(_screenview, 800, 600);

  var _height = height;
  var _width = width;
  var _palette = palette.map(function (c) {
    return c.toInt();
  });

  for (var i = 0; i < data.length; i++) {
    _wb[i] = _palette[data[i]];
  }

  console.log("palette", _palette);

  var PMASK = 15 << 24;

  var XMASK = 4095 << 12;
  var YMASK = 4095

  var INDEXMASK = XMASK | YMASK;

  var write = function (pack) {
    var x = (pack >> 12) & 4095;
    var y = pack & 4095;
    var p = pack >> 24;

    _wb[y * width + x] = _palette[p];

  };

  //bounds are checked before calling render
  var render = function (ctx, offsetx, offsety) {
    for (var j = 0; j < 600; j++) {
      var sj = offsety + j;

      for (var i = 0; i < 800; i++) {
        var si = offsetx + i;
        _screenwrite[j*800 + i] = _wb[sj*width + si];
      }
    }

    ctx.putImageData(_imageData, 0, 0);
  };

  return { write: write, render: render, width:_width, height:_height };


};