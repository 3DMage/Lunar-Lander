using LunarLander.ECS.Entities.UI_Items;
using LunarLander.Entities.GameObjects;
using LunarLander.Entities.UI_Items;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;

namespace LunarLander.Assets.Menu.GameUI
{
    // The display showing fuel, speed, and angle during gameplay.
    public class GameUI
    {
        // UI for showing how much fuel is left.
        UI_TextPair fuelLeft_UI;

        // UI for showing current speed of the lander.
        UI_TextPair currentSpeed_UI;

        // UI for showing current angle of the lander (in degrees).
        UI_TextPair currentAngle_UI;

        // Unit displays for each UI_Text Pair.
        UI_Text fuelUnits;
        UI_Text speedUnits;
        UI_Text angleUnits;

        // Reference to the lander.
        Lander lander;

        // Color for when the state of the lander is bad for landing.
        Color badColor = Color.White;

        // Color for when the state of the lander is good for landing.
        Color goodColor = new Color(115, 255, 33);

        // Constructor.
        public GameUI(Lander lander)
        {
            this.lander = lander;

            float offset_X = 0.88f;
            float offset_Y = 0.01f;
            int positionX = (int)(Managers.graphicsManager.WINDOW_WIDTH * offset_X);
            int positionY = (int)(Managers.graphicsManager.WINDOW_WIDTH * offset_Y);

            fuelLeft_UI = new UI_TextPair
            (
                positionX,
                positionY,
                UI_Origin.CENTER_LEFT,
                Managers.graphicsManager.spaceFont,
                badColor,
                10,
                "Fuel Left:",
                this.lander.fuel.ToString("F2")
            );

            fuelUnits = new UI_Text
            (
                positionX + (int)Managers.graphicsManager.spaceFont.MeasureString(fuelLeft_UI.rightText_UI.text).X,
                positionY,
                UI_Origin.CENTER_LEFT,
                Managers.graphicsManager.spaceFont,
                badColor,
                "  kg"
            );


            offset_Y += 0.02f;
            positionY = (int)(Managers.graphicsManager.WINDOW_WIDTH * offset_Y);

            currentSpeed_UI = new UI_TextPair
            (
                positionX,
                positionY,
                UI_Origin.CENTER_LEFT,
                Managers.graphicsManager.spaceFont,
                badColor,
                10,
                "Speed:",
                this.lander.CurrentSpeed().ToString("F2")
            );

            speedUnits = new UI_Text
            (
                positionX + (int)Managers.graphicsManager.spaceFont.MeasureString(currentSpeed_UI.rightText_UI.text).X,
                positionY,
                UI_Origin.CENTER_LEFT,
                Managers.graphicsManager.spaceFont,
                badColor,
                "  m/s"
            );

            offset_Y += 0.02f;
            positionY = (int)(Managers.graphicsManager.WINDOW_WIDTH * offset_Y);

            currentAngle_UI = new UI_TextPair
            (
                positionX,
                positionY,
                UI_Origin.CENTER_LEFT,
                Managers.graphicsManager.spaceFont,
                badColor,
                10,
                "Angle:",
                this.lander.DegreeRotation().ToString("F2")
            );

            angleUnits = new UI_Text
            (
                positionX + (int)Managers.graphicsManager.spaceFont.MeasureString(currentAngle_UI.rightText_UI.text).X,
                positionY,
                UI_Origin.CENTER_LEFT,
                Managers.graphicsManager.spaceFont,
                badColor,
                "  deg"
            );
        }

        // Updates the data in accordance to state of the lander.
        public void UpdateDisplayData()
        {
            // Display UI display.  Show only two decimal places.
            fuelLeft_UI.rightText_UI.SetText(lander.fuel.ToString("F2"));
            currentSpeed_UI.rightText_UI.SetText(lander.CurrentSpeed().ToString("F2"));
            currentAngle_UI.rightText_UI.SetText(lander.DegreeRotation().ToString("F2"));

            // Update colors if needed for each UI component.

            if (lander.fuel <= 0.0f)
            {
                fuelLeft_UI.rightText_UI.color = badColor;
                fuelUnits.color = badColor;
            }
            else
            {
                fuelLeft_UI.rightText_UI.color = goodColor;
                fuelUnits.color = goodColor;
            }

            // Get rotation of lander in degrees.
            float degreeRotation = lander.DegreeRotation();

            if (degreeRotation >= 355 && degreeRotation <= 360 || degreeRotation >= 0 && degreeRotation <= 5)
            {
                currentAngle_UI.rightText_UI.color = goodColor;
                angleUnits.color = goodColor;
            }
            else
            {
                currentAngle_UI.rightText_UI.color = badColor;
                angleUnits.color = badColor;
            }

            float speed = lander.CurrentSpeed();

            if (speed >= 0 && speed <= 2)
            {
                currentSpeed_UI.rightText_UI.color = goodColor;
                speedUnits.color = goodColor;
            }
            else
            {
                currentSpeed_UI.rightText_UI.color = badColor;
                speedUnits.color = badColor;
            }
        }

        // Draws the UI.
        public void Draw()
        {
            fuelLeft_UI.Draw_UI();
            currentSpeed_UI.Draw_UI();
            currentAngle_UI.Draw_UI();

            fuelUnits.Draw_UI();
            speedUnits.Draw_UI();
            angleUnits.Draw_UI();
        }
    }
}
