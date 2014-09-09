
function color(r, g, b, a) {
    this.r = r;
    this.g = g;
    this.b = b;
    this.a = a;
}

function bitmap(url) {

    var self = this;
    this.image = new Image();
    this.height = 0;
    this.width = 0;
    this.dataarray;
    this.imageData;
    this.ready = false;

    this.contextdata;
    this.parseCanvas;
    this.parseContext;

    this.image.src = url;

    this.image.onload = function () {

        self.width = self.image.width;
        self.height = self.image.height;

        self.parseCanvas = document.createElement('canvas');

        self.parseCanvas.width = self.width;
        self.parseCanvas.height = self.height;

        self.parseContext = self.parseCanvas.getContext('2d');
        

        self.parseContext.drawImage(self.image, 0, 0);
        self.imageData = self.parseContext.getImageData(0, 0, self.parseCanvas.width, self.parseCanvas.height);
        //var tempData = self.imageData.data;

        self.contextdata = self.imageData.data;

        self.dataarray = new Uint32Array(self.contextdata.buffer);

        self.ready = true;

    };


}

bitmap.prototype.setPixel = function (x, y, c) {

    if (this.ready) {

        x = Math.floor(x + .5);
        y = Math.floor(y + .5);



        var index = (x + y * this.width);

        /*
        var r = c.r & 0x000000ff;
        var g = c.g & 0x0000ff00;
        var b = c.b & 0x00ff0000;
        var a = c.a & 0xff000000;

        var tempi = 1;

        r = Math.floor(r * tempi);
        g = g >> 8;
        g = Math.floor(g * tempi);
        g = g << 8;
        b = b >> 16;
        b = Math.floor(b * tempi);
        b = b << 16;

        var combo = r | g | b | a;
        */


        var combo = (c.a << 24 | c.r << 16 | c.g << 8 | c.b << 0);

        //console.log(combo);

        this.dataarray[index] = combo;
           
        
        

    }



};

bitmap.prototype.getPixel = function (x, y) {



    

    //console.log("x :  " + x + "  y : " + y);



    var index = (y * this.width + x);   //*4;




    //var color = { r: brickData[index], g: brickData[index + 1], b: brickData[index + 2], a: 255 };

    return this.dataarray[index];

    //return color;



};

bitmap.prototype.redraw = function (context) {

    if (this.ready) {

        context.putImageData(this.imageData, 0, 0);
    }

};

bitmap.prototype.test = function (color) {

    console.log(this.getPixel(50, 50));
    
    this.setPixel(50, 50, color);
    console.log(this.getPixel(50, 50));
    
    for (var i = 50; i < 200; i++) {

        for (var j = 50; j < 200; j++) {

            this.setPixel(i, j, color);


        }


    }
    

};

