(function(window, document, $) {

    function Random(min, max) {
        return Math.random() * (max - min) + min;
    }

    function VectorTwo(X, Y) {

        this.x = X;
        this.y = Y;

    };

    VectorTwo.Copy = function (vec) {

        return new VectorTwo(vec.x, vec.y);

    };

    VectorTwo.Add = function(vec1, vec2){

        return new VectorTwo(vec1.x + vec2.x, vec1.y + vec2.y);

    };

    function Particle(position, velocity, color) {

        this._position = position;
        this._velocity = velocity;
        this._color = color;
        this._duration = 40;

    };

    Particle._indexArray = [];
    Particle._nextIndex = -1;
    Particle._particleArray = [];

    

    Particle.prototype.Refurbish = function (position, velocity, color) {

        this._position = position;
        this._velocity = velocity;
        this._color = color;
        this._duration = 40;

    };

    Particle.prototype.Update = function () {

        this._duration--;
        this._position = VectorTwo.Add(this._position, this._velocity);

        

    };

    Particle.Create = function (position, velocity, color) {

        if (Particle._nextIndex >= 0) {

            var temp = Particle._particleArray[Particle._indexArray[Particle._nextIndex]];
            Particle._nextIndex--;
            temp.Refurbish(position, velocity, color);
            //return temp;

        }
        else {

            Particle._particleArray.push(new Particle(position, velocity, color));
        }

    };

    Particle.Recycle = function (index) {


        Particle._nextIndex++;
        Particle._indexArray[Particle._nextIndex] = index;
        
        Particle._particleArray[index]._duration = -1;

    };

    function Emitter(position, velocity, particlevelocity) {

        this._position = position;
        this._velocity = velocity;
        this._particlevelocity = particlevelocity;
        this._duration = 5;
        this.parts = [];
    
        

        this.angle = Math.atan2(this._velocity.y, this._velocity.x);

    };

    Emitter.prototype.Update = function(){

        this._duration--;

        this._position = VectorTwo.Add(this._position, this._velocity);


        for (var i = 0; i < 10; i++) {

            var coneangle = this.angle + Random(-Math.PI / 32, Math.PI / 32);
            var intensity = Random(1, 2);

            //this.parts.push(new Particle(VectorTwo.Copy(this._position), new VectorTwo(Math.cos(coneangle), Math.sin(coneangle)),'red'));

            //particlearray.push(new Particle(VectorTwo.Copy(this._position), new VectorTwo(intensity * this._particlevelocity * Math.cos(coneangle), intensity * this._particlevelocity * Math.sin(coneangle)), 'red'));

            //particlearray.push(Particle.Create(VectorTwo.Copy(this._position), new VectorTwo(intensity * this._particlevelocity * Math.cos(coneangle), intensity * this._particlevelocity * Math.sin(coneangle)), 'red'));

            Particle.Create(VectorTwo.Copy(this._position), new VectorTwo(intensity * this._particlevelocity * Math.cos(coneangle), intensity * this._particlevelocity * Math.sin(coneangle)), 'red');

        }

    };

    function Projectile(id, position, velocity) {

        var self = this;
        this._id = id;
        this._position = position;
        this._velocity = velocity;


    };

    Projectile.prototype.Update = function () {

        this._position = VectorTwo.Add(this._position, this._velocity);


    };

    function Player() {

        this.id = -1;
        this.x = 0;
        this.y = 0;
        this.rotation = 0;
        this.velocity = 0;
        this.angularvelocity = 0;
        this.hit = false;

    };

    var wtfx = 5;
    var wtfy = 5;
    var myid = -1;

    var playerarray = [];
    var imagearray = [];
    var projectilearray = [];
    var emitterarray = [];
    var particlearray = Particle._particleArray;

    imagearray['avatar'] = new Image();
    imagearray['avatar'].src = 'img/person.png';

    var hub = $.connection.warlocksHub;

    $.connection.hub.start().done(function () {

        hub.server.startPlayer();
        Draw();

    });

    hub.client.hello = function () {


        var pageCoords = "( " + wtfx + ", " + wtfy + " )";
        $("span:first").text("( event.pageX, event.pageY ) : " + pageCoords);

    };

    hub.client.setId = function (arr) {


        myid = parseInt(arr);
        inputs.id = myid;

    };
    hub.client.updateProjectiles = function (projectiles) {

        //projectilearray = [];

        $.each(projectiles, function (index, proj) {

            if (typeof projectilearray[proj.id] == 'undefined') {
                // does not exist
                projectilearray[proj.id] = new Projectile( proj.id, new VectorTwo(parseFloat(proj.position.X), parseFloat(proj.position.Y)), new VectorTwo(parseFloat(proj.velocity.X), parseFloat(proj.velocity.Y)) );
                
            }
            else {
                // does exist
            }

            //projectilearray.push({});

            

            //projectilearray.push(proj);

        });

    };

    hub.client.addEmitter = function (position, velocity) {

        emitterarray.push(new Emitter(new VectorTwo(parseFloat(position.X), parseFloat(position.Y)), new VectorTwo(parseFloat(velocity.X), parseFloat(velocity.Y)), 4));

    };


    hub.client.updateState = function (players) {

        

        var newplayer;

        $.each(players, function (index, player) {

            newplayer = true;

            $.each(playerarray, function (index2, localplayer) {

                if (parseInt(player.id) == localplayer.id) {

                    localplayer.x = parseFloat(player.x);
                    localplayer.y = parseFloat(player.y);
                    localplayer.rotation = parseFloat(player.rotation);
                    newplayer = false;


                }

            });


            if (newplayer) {

                var tempplayer = new Player();

                tempplayer.id = parseInt(player.id);
                tempplayer.x = parseFloat(player.x);
                tempplayer.y = parseFloat(player.y);
                tempplayer.rotation = parseFloat(player.rotation);

                playerarray.push(tempplayer);

            }

        });

    };



    function playerinputs() {

        this.id = 0;
        this.mousex = 0;
        this.mousey = 0;
        this.rmb = 0;
        this.lmb = 0;
        this.q = 0;
        this.w = 0;
        this.e = 0;
        this.r = 0;

    };




    var input = {mousex:0, mousey:0, rmb: 0, lmb: 0, q:0, w:0, e:0, r:0};

    var inputs = new playerinputs();
   

    var canvas = document.getElementById('daggerctx');
    var context = canvas.getContext('2d');



    function Draw() {
        var requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame || window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;


        context.clearRect(0, 0, 1400, 800);
        context.fillStyle = "#7C6EFF";
        context.fillRect(0, 0, 555, 555);
        //context.drawImage(imagearray['avatar'], 1, 1);
        context.fillStyle = "black";

        $.each(playerarray, function (index, player) {
            
            //context.fillRect(player.x, player.y, 60, 60);
            context.save();
            context.translate(player.x, player.y);
            context.rotate(player.rotation);
            context.drawImage(imagearray['avatar'], -(15), -(15));

            context.restore();

        });

        context.fillStyle = "red";

        $.each(projectilearray, function (index, proj) {

            proj._position = VectorTwo.Add(proj._position, proj._velocity);

            context.fillRect(proj._position.x, proj._position.y, 20, 20);



        });

        for (var i = emitterarray.length - 1; i >= 0; i--) {

            var temp = emitterarray[i];

            if (temp._duration >= 0) {
                temp.Update();

            }

        }

        for (var i = particlearray.length - 1; i >= 0; i--) {

            var temp = particlearray[i];

            if (temp._duration > 0) {

                temp.Update();
                context.fillRect(temp._position.x-1, temp._position.y-1, 1, 1);
            }
            else if (temp._duration == 0) {
                
                Particle.Recycle(i);

            }


        }




        hub.server.sendInput(inputs);
        

        requestAnimationFrame(Draw);

        console.log("cached projectiles : " + Particle._nextIndex + "   active projectiles : " + Particle._particleArray.length + "  "  + particlearray.length);

    };






    context.mozImageSmoothingEnabled = false;
    context.webkitImageSmoothingEnabled = false;
    context.msImageSmoothingEnabled = false;
    context.imageSmoothingEnabled = false;

    $(document).bind("contextmenu", function (event) {
        event.preventDefault();



    });

    $(document).mousemove(function (event) {

        

        inputs.mousex = event.pageX - canvas.offsetLeft;
        inputs.mousey = event.pageY - canvas.offsetTop;


        wtfx = event.pageX;
        wtfy = event.pageY;


    });

    $(document).mousedown(function (e) {
        e.preventDefault()

        if (e.which > 1) {
            
            
            inputs.rmb = 1;
        }
        else {


            inputs.lmb = 1;

        }

        

        emitterarray.push(new Emitter(new VectorTwo(e.pageX, e.pageY), new VectorTwo(2,2), 4));

    });

    $(document).mouseup(function (e) {

        if (e.which > 1) {
            e.preventDefault()

            inputs.rmb = 0;
        }
        else {


            inputs.lmb = 0;

        }

    });


    $(document).keydown(function (e) {

        var keyid = e.which;

        switch (keyid) {
            case 81:
                inputs.q = 1;
                //hub.server.hello();
                break;
            case 87:
                inputs.w = 1;
                break;
            case 69:
                inputs.e = 1;
                break;
            case 82:
                inputs.r = 1;
                break;
            default: 
                    break;
        }

    });


    $(document).keyup(function (e) {

        var keyid = e.which;

        switch (keyid) {
            case 81:
                inputs.q = 0;
                break;
            case 87:
                inputs.w = 0;
                break;
            case 69:
                inputs.e = 0;
                break;
            case 82:
                inputs.r = 0;
                break;
            default:
                break;
        }

    });

    $(document).ready(function () {

        

    });


}(window, document, window.jQuery));