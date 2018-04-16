using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DXGrid.ColumnSync {
    public class ViewModel {
        public ViewModel() {
            Data = new ObservableCollection<TestData>();
            GenerateData(20);
        }

        private void GenerateData(int count) {
            for(int i = 0; i < count; i++)
                Data.Add(new TestData(i));
        }

        public ObservableCollection<TestData> Data { get; private set; }
    }

    public class TestData {
        public TestData() {
        }

        public TestData(int seed) {
            IntValue = seed;
            Text = "Text " + seed;
            DateValue = DateTime.Now.AddDays(-seed);
            SecondText = "Second Text " + seed;
            Details = new ObservableCollection<DetailData>();

            int detailCount = new Random().Next(5, 10);
            for(int i = 0; i < detailCount; i++)
                Details.Add(new DetailData(i + 100 + seed));
        }

        public int IntValue { get; set; }
        public string Text { get; set; }
        public DateTime DateValue { get; set; }
        public string SecondText { get; set; }
        public ObservableCollection<DetailData> Details { get; private set; }
    }

    public class DetailData {
        public DetailData(int seed) {
            DetailIntValue = seed;
            DetailText = "Detail Text " + seed;
            DetailDate = DateTime.Now.Date.AddDays(-seed);
            DetailBool = new Random().Next(0, 99) > 50;
            DetailSecondText = "Detail Second Text " + seed;
        }

        public int DetailIntValue { get; set; }
        public string DetailText { get; set; }
        public DateTime DetailDate { get; set; }
        public bool DetailBool { get; set; }
        public string DetailSecondText { get; set; }
    }
}
