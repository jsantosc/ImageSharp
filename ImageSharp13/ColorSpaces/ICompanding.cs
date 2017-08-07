﻿// <copyright file="ICompanding.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.ColorSpaces
{
    /// <summary>
    /// Pair of companding functions for <see cref="IRgbWorkingSpace"/>.
    /// Used for conversion to <see cref="CieXyz"/> and backwards.
    /// See also: <seealso cref="IRgbWorkingSpace.Companding"/>
    /// </summary>
    internal interface ICompanding
    {
        /// <summary>
        /// Expands a companded channel to its linear equivalent with respect to the energy.
        /// </summary>
        /// <remarks>
        /// For more info see:
        /// <see href="http://www.brucelindbloom.com/index.html?Eqn_RGB_to_XYZ.html"/>
        /// </remarks>
        /// <param name="channel">The channel value</param>
        /// <returns>The linear channel value</returns>
        float Expand(float channel);

        /// <summary>
        /// Compresses an uncompanded channel (linear) to its nonlinear equivalent (depends on the RGB color system).
        /// </summary>
        /// <remarks>
        /// For more info see:
        /// <see href="http://www.brucelindbloom.com/index.html?Eqn_XYZ_to_RGB.html"/>
        /// </remarks>
        /// <param name="channel">The channel value</param>
        /// <returns>The nonlinear channel value</returns>
        float Compress(float channel);
    }
}
