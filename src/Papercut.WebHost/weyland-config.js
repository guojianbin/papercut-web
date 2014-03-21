﻿exports.config = function (weyland) {
    weyland.build('main')
        .task.jshint({
            include: 'App/**/*.js'
        })
        .task.uglifyjs({
            include: ['App/**/*.js', 'Scripts/durandal/**/*.js']
        })
        .task.rjs({
            include: ['App/**/*.{js,html}', 'Scripts/durandal/**/*.js'],
            loaderPluginExtensionMaps: {
                '.html': 'text'
            },
            rjs: {
                name: '../Scripts/almond-custom', //to deploy with require.js, use the build's name here instead
                insertRequire: ['main'], //not needed for require
                baseUrl: 'App',
                wrap: true, //not needed for require
                paths: {
                    'text': '../Scripts/text',
                    'durandal': '../Scripts/durandal',
                    'plugins': '../Scripts/durandal/plugins',
                    'transitions': '../Scripts/durandal/transitions',
                    'knockout': '../Scripts/knockout-2.3.0',
                    'bootstrap': '../Scripts/bootstrap',
                    'jquery': '../Scripts/jquery-1.9.1',
                    'moment': '../Scripts/moment-2.0.0'
                },
                inlineText: true,
                optimize: 'none',
                pragmas: {
                    build: true
                },
                stubModules: ['text'],
                keepBuildDir: true,
                out: 'App/main-built.js'
            }
        });
}