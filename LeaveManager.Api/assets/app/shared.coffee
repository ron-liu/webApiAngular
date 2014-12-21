angular.module 'app.shared', []

.directive 'submitValid', ['$parse', '$q', ($parse, $q) ->
	require: 'form'
	link: (scope, formElement, attributes, form) ->
		form.attempted = false
		formElement.bind 'submit', (event) ->
			scope.$apply  -> form.loading = true
			form.attempted = true
			if  not scope.$$phase then scope.$apply()
			fn = $parse attributes.submitValid
			fromForm = form.from ? form
			if (fromForm.$valid)
				scope.$apply  ->
					$q.when(fn scope, $event:event)
					.finally ->
						form.loading = false
						form.$setPristine()
						form.attempted = false
			else scope.$apply  -> form.loading = false
]

.directive 'gtValidator', ->
	require: 'ngModel'
	link: (scope, element, attrs, ngModel) ->
		ngModel.$validators.gt = (value) ->
			compareTo = scope.$eval attrs.gtValidator
			if compareTo then value > compareTo else true

.directive 'ltValidator', ->
	require: 'ngModel'
	link: (scope, element, attrs, ngModel) ->
		ngModel.$validators.lt = (value) ->
			compareTo = scope.$eval attrs.gtValidator
			if compareTo then value < compareTo	else true

.directive 'fromNow', ['$parse', '$compile', ($parse, $compile) ->
	restrict: 'EA'
	compile: (ele, attrs) ->
		p = attrs.ngBind

		(scope, element, attrs) ->
			template = ''
			if attrs.showDetail is 'true'
				template = "<span ng-if='#{p}' ng-bind-template=\"{{moment(#{p}).fromNow()}}, {{#{p} | date: 'dd/MM/yyyy HH:mm'}}\"></span> "
			else
				template = "<a ng-if='#{p}' href='javascript:' tooltip=\"{{#{p} | date:'dd/MM/yyyy HH:mm'}}\" ><span ng-bind='moment(#{p}).fromNow()'></span></a>"
			element.replaceWith($compile(template) scope)
]