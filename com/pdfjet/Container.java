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

    public Container(float width, float height) {
        this.width = width;
        this.height = height;
        this.rotateDegrees = 0f;
        this.scaleX = 1f;
        this.scaleY = 1f;
        this.elements = new ArrayList<Drawable>();
    }

    /** Sets the position of the container on the page. */
    public void setPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }

    /** Sets the location of the container on the page (alias for setPosition). */
    public void setLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    /** Sets the rotation angle (in degrees). */
    public void setRotation(double degrees) {
        this.rotateDegrees = (float) degrees;
    }

    /** Sets clockwise rotation by the given angle (in degrees). */
    public void setRotationClockwise(double degrees) {
        this.rotateDegrees = (float) -degrees;
    }

    /** Sets counter-clockwise rotation by the given angle (in degrees). */
    public void setRotationCounterClockwise(double degrees) {
        this.rotateDegrees = (float) degrees;
    }

    /** Sets uniform scaling for both X and Y axes. */
    public void setScaleFactor(float factor) {
        setScaleFactorXY(factor, factor);
    }

    /** Sets non-uniform scaling factors for X and Y axes. */
    public void setScaleFactorXY(float sx, float sy) {
        this.scaleX = sx;
        this.scaleY = sy;
    }

    /** Adds a drawable element to this container. */
    public void add(Drawable element) {
        this.elements.add(element);
    }

    /** Draws this container and its child elements onto the page. */
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
