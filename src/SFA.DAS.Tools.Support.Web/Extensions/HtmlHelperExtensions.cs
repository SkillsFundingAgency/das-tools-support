﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class HtmlHelperExtensions
{
    public static HtmlString AddClassIfPropertyInError<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression,
        string errorClass)
    {
        var expressionProvider = htmlHelper.ViewContext.HttpContext.RequestServices
            .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;

        var expressionText = expressionProvider?.GetExpressionText(expression);
        var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
        var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

        if (state?.Errors == null || state.Errors.Count == 0)
        {
            return HtmlString.Empty;
        }

        return new HtmlString(errorClass);
    }
}