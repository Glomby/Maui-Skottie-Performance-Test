using System.Collections.ObjectModel;
using EvergineIntegrationApp.Models;

namespace EvergineIntegrationApp.Views.Setup.Guides
{
    public static class Guides
    {
        public static ObservableCollection<GuidesItem> FirstStepsCollection { get; } = new() 
        {
            new GuidesItem(){
                HeroBanner = "AdapterToConsole.json",
                Header = "Guide Item 1",
                Text = "Guide item text. Animation should be playing right now as smoothly as possible",
                ButtonText = "Next",
                BackButtonVisible = true },

            new GuidesItem(){
                HeroBanner = "ControllerToAdapter.json",
                Header = "Guide Item 2",
                Text = "Guide item text. Animation should be playing right now as smoothly as possible",
                ButtonText ="Next",
                BackButtonVisible = true }
        };

        public static List<GuidesItem> FirstSteps = new List<GuidesItem>()
        {
            new GuidesItem(){
                HeroBanner = "AdapterToConsole.json",
                Header = "Guide Item 1",
                Text = "Guide item text. Animation should be playing right now as smoothly as possible",
                ButtonText = "Next",
                BackButtonVisible = true },

            new GuidesItem(){
                HeroBanner = "ControllerToAdapter.json",
                Header = "Guide Item 2",
                Text = "Guide item text. Animation should be playing right now as smoothly as possible",
                ButtonText ="Next",
                BackButtonVisible = true }
        };
    }
}
