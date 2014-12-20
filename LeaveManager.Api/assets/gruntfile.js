/// <vs BeforeBuild='build' />
module.exports = function(grunt) {

    require('load-grunt-tasks')(grunt);

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        compass: {
            options: {
                sassDir: './styles/',
                cssDir: './styles/',
                trace: true,
                force: true,
                raw: 'Encoding.default_external = \'utf-8\'\n'
            },
            dist: {
                options: {
                }
            }
        },
        coffee: {
            options: {
                sourceMap: false
            },
            dist: {
                files: [
                {
                    expand: true,
                    cwd: './app/',
                    src: ['**/*.coffee'],
                    dest: './app/',
                    ext: '.js',
                    extDot: 'last'
                }]
            }
        },
        karma: {
            unit: {
                configFile: './test/karma.conf.js'
            }
        },
		watch: {
			scss: {
				files: '**/*.scss',
				tasks: ['compass:dist'],
				options: {
					debounceDelay: 500
				}
			},
			coffee: {
				files: '**/*.coffee' ,
				tasks: ['coffee:dist'] ,
				options: {
					debounceDelay: 500
				}
			},
            karma:{
                files: './test/**/*.js',
                tasks: ['karma:unit'],
                options: {
                    debounceDelay: 500
                }
            }
		},
		concurrent: {
			options: {
				logConcurrentOutput: true
			},
			ww: {
				tasks: ["watch:scss", "watch:coffee", "watch:karma"]
			}
		}

    });

    grunt.registerTask('build', [
        'compass:dist',
        'coffee:dist',
    ]);

    grunt.registerTask('default', ['build']);

	grunt.registerTask('w', ['concurrent:ww'])

};