function GetInput(controller) {

  if (navigator.getGamepads) {

    var tolerance = .3;

    controller.dleft = 0;
    controller.dright = 0;
    controller.dup = 0;
    controller.ddown = 0;
    //controller.rightx = 0;
    //controller.righty = 0;
    controller.righttrigger = 0;
    controller.buttons[0] = 0;
    controller.buttons[1] = 0;
    controller.buttons[2] = 0;
    controller.buttons[3] = 0;
    controller.buttons[4] = 0;

    var gamepadarray = navigator.getGamepads();



    if (gamepadarray.length > 0) {

      var gamepad = gamepadarray[0];

      //console.log(gamepad.mapping);

      var buttons = gamepad.buttons;
      var axes = gamepad.axes;

      if (gamepad.axes.length < 6) {


        //chrome reads dpad as buttons
        controller.ddown = buttons[13].value;
        controller.dup = buttons[12].value;
        controller.dleft = buttons[14].value;
        controller.dright = buttons[15].value;


        if (buttons[7].value >= tolerance) {
          controller.righttrigger = 1;
          controller.buttons[4] = 1;
        }

        //chrome reads x and y from different index then firefox

        /*
        if (Math.abs(axes[2]) > tolerance) {
            controller.rightx = axes[2];
        }
        if (Math.abs(axes[3]) > tolerance) {
            controller.righty = axes[3];
        }
        */

        if (axes[2] * axes[2] + axes[3] * axes[3] > tolerance) {
          controller.rightx = axes[2];
          controller.righty = axes[3];
        }



      }
      else {
        //firefox reads dpad as axes

        if (axes[5] == -1) {
          controller.dleft = 1;
        }
        else if (axes[5] == 1) {
          controller.dright = 1;

        }
        if (axes[6] == -1) {
          controller.dup = 1;

        }
        else if (axes[6] == 1) {
          controller.ddown = 1;
        }

        if (axes[4] <= -tolerance) {
          controller.righttrigger = 1;
          controller.buttons[4] = 1;
        }
        /*
        if (Math.abs(axes[3]) > tolerance) {
            controller.rightx = axes[3];
        }
        if (Math.abs(axes[2]) > tolerance) {
            controller.righty = axes[2];
        }
        */
        if (axes[2] * axes[2] + axes[3] * axes[3] > tolerance) {
          controller.rightx = axes[3];
          controller.righty = axes[2];
        }

      }

      for (var i = 0; i < 4; i++) {

        controller.buttons[i] = buttons[i].value;

      }

      /*
      for (var i = 0; i < gamepadarray[0].buttons.length; i++) {
          if (gamepadarray[0].buttons[i].value > 0) {
              console.log(i + "   " + gamepadarray[0].buttons[i].value);
          }

      }
      */

    }
    else {
      console.log("no gamepad detected");
    }

  }


}