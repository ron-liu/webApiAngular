angular.module 'app.leave', ['ui.router', 'ui.bootstrap', 'app.shared', 'app.security']

.config ['$stateProvider', ($stateProvider) ->
	$stateProvider
	.state 'home.dashboard',
		url: 'dashboard'
		templateUrl: 'app/dashboard.html'
		controller: ['$scope', 'stat', ($scope, stat)->
			$scope.stat = stat
		]
		resolve:
			stat: ['LeaveService', (LeaveService) -> LeaveService.stat()]

	.state 'home.apply',
		url: 'apply'
		templateUrl: 'app/apply.html'
		controller: ['options','$scope','LeaveService', '$state', 'NotificationManager', '$timeout', (options, $scope, LeaveService, $state, NotificationManager, $timeout)->
			$scope.options = options
			$scope.model = {}
			$scope.submit = (model) ->
				LeaveService.apply model
				.then ->
					NotificationManager.setMessages [MessageType: 'Info', Content: 'Submit successfully, redirecting to list page.']
					$timeout (-> $state.go 'home.list'), 2000


			getWorkingDays =  (start, end)->
				if not start? or not end? then return
				LeaveService.getWorkingDays start, end
				.then (res) ->
					$scope.model.duration = res

			$scope.$watch 'model.StartDate', (newVal, oldVal) -> getWorkingDays newVal, $scope.model.EndDate
			$scope.$watch 'model.EndDate', (newVal) -> getWorkingDays $scope.model.StartDate, newVal

		]
		resolve:
			options : ['LeaveService', (LeaveService) -> LeaveService.options()]

	.state 'home.list',
		url: 'list'
		templateUrl: 'app/listMyLeaves.html'
		controller: ['leaves','$scope', (leaves, $scope)->
			$scope.leaves = leaves
		]
		resolve:
			leaves: ['LeaveService', (LeaveService) -> LeaveService.listMyLeaves() ]

	.state 'home.viewMyLeave',
		url: 'my-leave/:leaveId'
		templateUrl: 'app/viewMyLeave.html'
		controller: ['leave', '$scope', (leave, $scope) ->
			$scope.leave = leave
		]
		resolve:
			leave: ['LeaveService', '$stateParams', (LeaveService, $stateParams) -> LeaveService.getMineById $stateParams.leaveId  ]

	.state 'home.listEvaluate',
		url: 'list-evaluate'
		templateUrl: 'app/listToEvaluate.html'
		controller: ['leaves', '$scope', (leaves, $scope)->
			$scope.leaves = leaves
		]
		resolve:
			leaves: ['LeaveService', (LeaveService) -> LeaveService.listToEvaluate()]

	.state 'home.evaluate',
		url: 'evaluate/:leaveId'
		templateUrl: 'app/evaluate.html'
		controller: ['leave', '$scope','LeaveService','AuthDataStorage', '$state', 'NotificationManager', '$timeout', (leave, $scope, LeaveService, AuthDataStorage, $state, NotificationManager, $timeout)->
			$scope.leave = leave
			$scope.submit = (model) ->
				LeaveService.evaluate _.extend model, leaveId: leave.Id, UserName: AuthDataStorage.getUser()
				.then ->
					NotificationManager.setMessages [MessageType: 'Info', Content: 'Submit successfully, redirecting to list page.']
					$timeout (-> $state.go 'home.listEvaluate'), 1000
		]
		resolve:
			leave: ['LeaveService', '$stateParams', (LeaveService, $stateParams) -> LeaveService.getById($stateParams.leaveId)]
]

.factory 'LeaveService', [ 'Restangular', (Restangular)->
	listMyLeaves: -> Restangular.all('list-my-leaves').getList()
	apply: (model) -> Restangular.all('apply').post model
	listToEvaluate: -> Restangular.all('list-to-evaluate').getList()
	getById: (leaveId) -> Restangular.one('leave', leaveId).get()
	getMineById: (leaveId) -> Restangular.one('my-leave', leaveId).get()
	evaluate: (model) -> Restangular.all('evaluate').post _.extend model
	getWorkingDays: (start, end) -> Restangular.all('working-days').post StartDate: start, EndDate: end
	stat : -> Restangular.one('stat').get()

	options: ->Restangular.one('options').get()
]
