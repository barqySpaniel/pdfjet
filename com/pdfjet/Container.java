package com.pdfjet;

import java.util.ArrayList;
import java.util.List;

public class Container implements Drawable {
    public float x;
    public float y;
    public float width;
    public float height;
    public float rotateDegrees;
    public float scaleX;
    public float scaleY;
    private List<Drawable> elements;

    /**
     * Creates a new container with the specified width and height.
     * <p>
     * The container is initialized with:
     * <ul>
     *   <li>Rotation set to {@code 0} degrees</li>
     *   <li>Scaling factors set to {@code 1.0} for both axes</li>
     *   <li>An empty list of drawable elements</li>
     * </ul>
     *
     * @param width  the width of the container
     * @param height the height of the container
     */
    public Container(float width, float height) {
        this.width = width;
        this.height = height;
        this.rotateDegrees = 0f;
        this.scaleX = 1f;
        this.scaleY = 1f;
        this.elements = new ArrayList<Drawable>();
    }

    /**
     * Sets the position of the container on the page.
     *
     * @param x the X coordinate
     * @param y the Y coordinate
     */
    public void setPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }

    /**
     * Sets the location of the container on the page (alias for {@link #setPosition}).
     *
     * @param x the X coordinate
     * @param y the Y coordinate
     */
    public void setLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    /**
     * Sets the rotation angle.
     *
     * @param degrees the rotation angle in degrees
     */
    public void setRotation(double degrees) {
        this.rotateDegrees = (float) degrees;
    }

    /**
     * Sets clockwise rotation.
     *
     * @param degrees the rotation angle in degrees (clockwise)
     */
    public void setRotationClockwise(double degrees) {
        this.rotateDegrees = (float) -degrees;
    }

    /**
     * Sets counter-clockwise rotation.
     *
     * @param degrees the rotation angle in degrees (counter-clockwise)
     */
    public void setRotationCounterClockwise(double degrees) {
        this.rotateDegrees = (float) degrees;
    }

    public float[] getRotationCenter() {
        return new float[] {x + width/2f, y + height/2f};
    }

    /**
     * Sets a uniform scaling factor for both X and Y axes.
     *
     * @param factor the scaling factor to apply
     */
    public void setScaleFactor(float factor) {
        setScaleFactorXY(factor, factor);
    }

    /**
     * Sets non-uniform scaling factors for the X and Y axes.
     *
     * @param sx the scaling factor for X
     * @param sy the scaling factor for Y
     */
    public void setScaleFactorXY(float sx, float sy) {
        this.scaleX = sx;
        this.scaleY = sy;
    }

    /**
     * Adds a drawable element to this container.
     *
     * @param element the element to add
     */
    public void add(Drawable element) {
        this.elements.add(element);
    }

    protected static float[] rotateAroundCenter(float[] point, float[] center, double degrees) {
        double rad = degrees * Math.PI / 180.0; // convert to radians

        // translate to centre
        double dx = (double) (point[0] - center[0]);
        double dy = (double) (point[1] - center[1]);

        // rotate
        double cos = Math.cos(rad);
        double sin = Math.sin(rad);
        double dxRot =  dx * cos - dy * sin;
        double dyRot =  dx * sin + dy * cos;

        // translate back
        double nx = center[0] + dxRot;
        double ny = center[1] + dyRot;

        return new float[] {(float) nx, (float) ny};
    }

    /**
     * Draws this container and its child elements onto the page.
     *
     * @param page the {@link Page} to draw on
     * @return an array containing the bottom-right position of the container
     * @throws Exception if drawing fails
     */
    public float[] drawOn(Page page) throws Exception {
        page.append("q\n"); // Save the graphics state

        page.append("1 0 0 1 ");
        page.append(this.x);
        page.append(' ');
        page.append(-this.y);
        page.append(" cm\n");

        float cx = width / 2f;
        float cy = height / 2f;

        page.append("1 0 0 1 ");
        page.append(cx);
        page.append(' ');
        page.append(page.height - cy);
        page.append(" cm\n");

        double rad = rotateDegrees * (Math.PI / 180.0);
        float cos = (float)Math.cos(rad);
        float sin = (float)Math.sin(rad);
        page.append(FastFloat.toByteArray(cos));
        page.append(' ');
        page.append(FastFloat.toByteArray(sin));
        page.append(' ');
        page.append(FastFloat.toByteArray(-sin));
        page.append(' ');
        page.append(FastFloat.toByteArray(cos));
        page.append(" 0 0 cm\n");

        page.append(scaleX);
        page.append(' ');
        page.append('0');
        page.append(' ');
        page.append('0');
        page.append(' ');
        page.append(scaleY);
        page.append(' ');
        page.append('0');
        page.append(' ');
        page.append('0');
        page.append(" cm\n");

        page.append("1 0 0 1 ");
        page.append(-cx);
        page.append(' ');
        page.append(-(page.height - cy));
        page.append(" cm\n");

        for (Drawable element : elements) {
            element.drawOn(page);
        }

        page.append("Q\n"); // Restore the graphics state

        return new float[] { this.x + width, this.y + height };
    }
}
