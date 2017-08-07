﻿// <copyright file="VonKriesChromaticAdaptation.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.ColorSpaces.Conversion
{
    using System.Numerics;

    using ImageSharp.ColorSpaces;
    using ImageSharp.ColorSpaces.Conversion.Implementation.Lms;

    /// <summary>
    /// Basic implementation of the von Kries chromatic adaptation model
    /// </summary>
    /// <remarks>
    /// Transformation described here:
    /// http://www.brucelindbloom.com/index.html?Eqn_ChromAdapt.html
    /// </remarks>
    internal class VonKriesChromaticAdaptation : IChromaticAdaptation
    {
        private readonly CieXyzAndLmsConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        public VonKriesChromaticAdaptation()
            : this(new CieXyzAndLmsConverter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        /// <param name="transformationMatrix">
        /// The transformation matrix used for the conversion (definition of the cone response domain).
        /// <see cref="LmsAdaptationMatrix"/>
        /// </param>
        public VonKriesChromaticAdaptation(Matrix4x4 transformationMatrix)
            : this(new CieXyzAndLmsConverter(transformationMatrix))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        /// <param name="converter">The color converter</param>
        public VonKriesChromaticAdaptation(CieXyzAndLmsConverter converter)
        {
            this.converter = converter;
        }

        /// <inheritdoc/>
        public CieXyz Transform(CieXyz sourceColor, CieXyz sourceWhitePoint, CieXyz targetWhitePoint)
        {
            Guard.NotNull(sourceColor, nameof(sourceColor));
            Guard.NotNull(sourceWhitePoint, nameof(sourceWhitePoint));
            Guard.NotNull(targetWhitePoint, nameof(targetWhitePoint));

            if (sourceWhitePoint.Equals(targetWhitePoint))
            {
                return sourceColor;
            }

            Lms sourceColorLms = this.converter.Convert(sourceColor);
            Lms sourceWhitePointLms = this.converter.Convert(sourceWhitePoint);
            Lms targetWhitePointLms = this.converter.Convert(targetWhitePoint);

            var vector = new Vector3(targetWhitePointLms.L / sourceWhitePointLms.L, targetWhitePointLms.M / sourceWhitePointLms.M, targetWhitePointLms.S / sourceWhitePointLms.S);
            var targetColorLms = new Lms(Vector3.Multiply(vector, sourceColorLms.Vector));

            return this.converter.Convert(targetColorLms);
        }
    }
}