export class ProductModel {
    constructor(
        public title: string = "",
        public description: string = "",
        public price: number = 0,
        public thumbnail: string = "",
        public stock: number = 0,
        public id: number = 0
    ){
       
    }

}