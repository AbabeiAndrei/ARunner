/// <binding BeforeBuild='all' ProjectOpened='watch:tasks' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        clean: ['wwwroot/lib/*', 'temp'],
        concat: {
            all: {
                src: [
                    'wwwroot/app/Init.js',
                    'wwwroot/app/services/*.js',
                    'wwwroot/app/filters/*.js',
                    'wwwroot/app/directives/*.js',
                    'wwwroot/app/layoutController.js',
                    'wwwroot/app/controllers/*.js',
                    'wwwroot/app/controllers/modals/*.js'
                ],
                dest: 'wwwroot/app/app.js'
            }
        },
        watch: {
            files: [
                'wwwroot/app/Init.js',
                'wwwroot/app/services/*.js',
                'wwwroot/app/filters/*.js',
                'wwwroot/app/directives/*.js',
                'wwwroot/app/layoutController.js',
                'wwwroot/app/controllers/*.js',
                'wwwroot/app/controllers/modals/*.js'
            ],
            tasks: ['all']
        }
    });


    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-watch'); 
    grunt.registerTask('all', ['clean', 'concat'/*, 'jshint', 'uglify'*/]);
};