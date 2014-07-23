(function(window, document, $) {

    function Player() {

        this.id = -1;
        this.x = 0;
        this.y = 0;
        this.rotation = 0;
        this.velocity = 0;
        this.angularvelocity = 0;

    };

    var wtfx = 5;
    var wtfy = 5;
    var myid = -1;

    var playerarray = [];
    var imagearray = [];

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

    hub.client.updateState = function (players) {

        //console.log(players);

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
            console.log(player.rotation + " ,  "  + player.x + " ,  " + player.y);
            //context.fillRect(player.x, player.y, 60, 60);
            context.save();
            context.translate(player.x, player.y);
            context.rotate(player.rotation);
            context.drawImage(imagearray['avatar'], -(15), -(15));

            context.restore();

        });

        

        hub.server.sendInput(inputs);
        //console.log(inputs);

        requestAnimationFrame(Draw);

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