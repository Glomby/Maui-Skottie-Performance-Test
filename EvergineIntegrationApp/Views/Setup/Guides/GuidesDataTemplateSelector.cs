using EvergineIntegrationApp.Models;
using EvergineIntegrationApp.Views.Setup.Guides.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace EvergineIntegrationApp.Views.Setup.Guides
{
    public class GuidesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GuideStepItem { get; set; }

        public GuidesDataTemplateSelector()
        {
            GuideStepItem = new DataTemplate(typeof(FirstStepsTemplate));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            GuidesItem currentItem = (GuidesItem)item;
            
            switch (currentItem.GuideDataTemplate) 
            {
                case GuideDataTemplate.Default:
                    return GuideStepItem;

                default: 
                    return GuideStepItem;

            }
            
        }
    }
}
